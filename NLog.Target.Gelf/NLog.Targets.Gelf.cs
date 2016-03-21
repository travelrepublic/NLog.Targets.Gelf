using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using NLog.Config;

namespace NLog.Targets.Gelf
{
    [Target("Gelf")]
    public sealed class GelfTarget : Target
    {
        #region Private Members

        private static readonly Socket SocketClient = new Socket(SocketType.Dgram, ProtocolType.Udp);
        private static readonly ConcurrentDictionary<string, IPEndPoint> EndPoints = new ConcurrentDictionary<string, IPEndPoint>();

        private const int ShortMessageLength = 250;
        private const int MaxMessageIdSize = 8;
        private const int MaxNumberOfChunksAllowed = 128;

        #endregion

        #region Public properties

        [RequiredParameter]
        public string GelfServer { get; set; }

        public int Port { get; set; }
        public string Facility { get; set; }
        public int MaxChunkSize { get; set; }

        #endregion

        #region Public Constructors

        public GelfTarget()
        {
            GelfServer = "127.0.0.1";
            Port = 12201;
            Facility = null;
            MaxChunkSize = 1024;
        }

        #endregion

        #region Overridden NLog methods

        /// <summary>
        /// This is where we hook into NLog, by overriding the Write method. 
        /// </summary>
        /// <param name="logEvent">The NLog.LogEventInfo </param>
        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                SendMessage(GelfServer, Port, CreateGelfJsonFromLoggingEvent(logEvent));
            }
            catch (Exception exception)
            {
                // If there's an error then log the message.
                SendMessage(GelfServer, Port, CreateFatalGelfJson(exception));
            }
        }

        #endregion

        #region Private Methods

        private void SendMessage(string gelfServer, int serverPort, string message)
        {
            var endPoint = GetIPEndPoint(gelfServer, serverPort);

            var gzipMessage = GzipMessage(message);
            if (gzipMessage.Length > MaxChunkSize)
            {
                var chunkCount = (gzipMessage.Length / MaxChunkSize) + 1;
                if (chunkCount > MaxNumberOfChunksAllowed)
                    return;

                var messageId = GenerateMessageId();
                for (var i = 0; i < chunkCount; i++)
                {
                    var messageChunkPrefix = CreateChunkedMessagePart(messageId, i, chunkCount);
                    var skip = i * MaxChunkSize;
                    var messageChunkSuffix = gzipMessage.Skip(skip).Take(MaxChunkSize).ToArray();

                    var messageChunkFull = new byte[messageChunkPrefix.Length + messageChunkSuffix.Length];
                    messageChunkPrefix.CopyTo(messageChunkFull, 0);
                    messageChunkSuffix.CopyTo(messageChunkFull, messageChunkPrefix.Length);

                    SocketClient.SendTo(messageChunkFull, 0, messageChunkFull.Length, SocketFlags.None, endPoint);
                }
            }
            else
            {
                SocketClient.SendTo(gzipMessage, 0, gzipMessage.Length, SocketFlags.None, endPoint);
            }
        }

        private static IPEndPoint GetIPEndPoint(string gelfServer, int serverPort)
        {
            return EndPoints.GetOrAdd(gelfServer,
                                      s =>
                                          {
                                              var hostAddress = Dns.GetHostAddresses(gelfServer).FirstOrDefault();

                                              return hostAddress == null ? null : new IPEndPoint(hostAddress, serverPort);
                                          });
        }

        private static byte[] GzipMessage(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var stream = new MemoryStream();

            using (var gZipStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }
            stream.Position = 0;

            var compressed = new byte[stream.Length];
            stream.Read(compressed, 0, compressed.Length);

            return compressed;
        }

        private string CreateGelfJsonFromLoggingEvent(LogEventInfo logEventInfo)
        {
            var shortMessage = logEventInfo.FormattedMessage.Length > ShortMessageLength ? logEventInfo.FormattedMessage.Substring(0, ShortMessageLength - 1) : logEventInfo.FormattedMessage;

            var gelfMessage = new GelfMessage
                {
                    Facility = Facility ?? "GELF",
                    FullMessage = logEventInfo.FormattedMessage,
                    Host = Dns.GetHostName(),
                    Level = logEventInfo.Level.GelfSeverity(),
                    ShortMessage = shortMessage,
                    Logger = logEventInfo.LoggerName ?? ""
                };

            if (logEventInfo.Properties != null)
            {
                object notes;
                if (logEventInfo.Properties.TryGetValue("Notes", out notes))
                {
                    gelfMessage.Notes = (string) notes;
                }
            }

            if (logEventInfo.Exception == null) return JsonConvert.SerializeObject(gelfMessage);

            var exceptioToLog = logEventInfo.Exception;

            while (exceptioToLog.InnerException != null)
            {
                exceptioToLog = exceptioToLog.InnerException;
            }

            gelfMessage.ExceptionType = exceptioToLog.GetType().Name;
            gelfMessage.ExceptionMessage = exceptioToLog.Message;
            gelfMessage.StackTrace = exceptioToLog.StackTrace;

            return JsonConvert.SerializeObject(gelfMessage);
        }

        private string CreateFatalGelfJson(Exception exception)
        {
            var gelfMessage = new GelfMessage
                {
                    Facility = Facility ?? "GELF",
                    FullMessage = "Error sending message in NLog.Targets.Gelf",
                    Host = Dns.GetHostName(),
                    Level = LogLevel.Fatal.GelfSeverity(),
                    ShortMessage = "Error sending message in NLog.Targets.Gelf"
                };

            if (exception == null) return JsonConvert.SerializeObject(gelfMessage);

            var exceptioToLog = exception;

            while (exceptioToLog.InnerException != null)
            {
                exceptioToLog = exceptioToLog.InnerException;
            }

            gelfMessage.ExceptionType = exceptioToLog.GetType().Name;
            gelfMessage.ExceptionMessage = exceptioToLog.Message;
            gelfMessage.StackTrace = exceptioToLog.StackTrace;

            return JsonConvert.SerializeObject(gelfMessage);
        }

        private static byte[] CreateChunkedMessagePart(string messageId, int chunkNumber, int chunkCount)
        {
            //Chunked GELF ID: 0x1e 0x0f (identifying this message as a chunked GELF message)
            var result = new List<byte>
                {
                    Convert.ToByte(30),
                    Convert.ToByte(15)
                };

            //Message ID: 32 bytes
            result.AddRange(Encoding.Default.GetBytes(messageId));

            result.AddRange(GetChunkPart(chunkNumber, chunkCount));

            return result.ToArray<byte>();
        }

        private static IEnumerable<byte> GetChunkPart(int chunkNumber, int chunkCount)
        {
            return new List<byte>
                {
                    Convert.ToByte(chunkNumber),
                    Convert.ToByte(chunkCount)
                };
        }

        private static string GenerateMessageId()
        {
            var random = new Random((int) DateTime.Now.Ticks);
            var r = random.Next(10000000).ToString("00000000");

            //Message ID: 8 bytes
            return r.Substring(0, MaxMessageIdSize);
        }

        #endregion
    }
}
