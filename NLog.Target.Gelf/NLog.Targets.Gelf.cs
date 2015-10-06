using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace NLog.Targets
{
    [Target("Gelf")]
    public sealed class Gelf : Target
    {
        #region Private Members

        private const int ShortMessageLength = 250;
        private const string GelfVersion = "1.1";

        private readonly CultureInfo _cultureInfoGB = new CultureInfo("en-GB");

        private int MaxHeaderSize
        {
            get
            {
                switch (GraylogVersion)
                {
                    case "0.9.6":
                        return 8;
                    default: // Default to version "0.9.5".
                        return 32;
                }
            }
        }

        #endregion

        #region Public properties

        public string GelfServer { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string Facility { get; set; }
        public int MaxChunkSize { get; set; }
        public string GraylogVersion { get; set; }

        #endregion

        #region Public Constructors

        public Gelf()
        {
            GelfServer = "127.0.0.1";
            Port = 12201;
            Sender = Assembly.GetCallingAssembly().GetName().Name;
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
            // Store the current UI culture
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            // Set the current Locale to "en-GB" for proper date formatting
            Thread.CurrentThread.CurrentCulture = _cultureInfoGB;

            var message = CreateGelfJsonFromLoggingEvent(logEvent.FormattedMessage, logEvent.Exception, logEvent.Level);
            try
            {
                SendMessage(GelfServer, Port, message);
            }
            catch (Exception e)
            {
                // If there's an error then log the message.
                var errorMessage = CreateGelfJsonFromLoggingEvent(e.ToString(), e, LogLevel.Fatal);
                SendMessage(GelfServer, Port, errorMessage);
            }

            // Restore the original culture
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        #endregion

        #region Private Methods

        private void SendMessage(string gelfServer, int serverPort, string message)
        {
            var hostAddress = Dns.GetHostAddresses(gelfServer).FirstOrDefault();

            if (hostAddress == null) return;

            var ipAddress = hostAddress.ToString();
            using (var udpClient = new UdpClient(ipAddress, serverPort))
            {
                var gzipMessage = GzipMessage(message);
                if (gzipMessage.Length > MaxChunkSize)
                {
                    var chunkCount = (gzipMessage.Length / MaxChunkSize) + 1;
                    if (chunkCount > 127)
                        throw new InvalidOperationException("The number of chunks to send was greater than 127.");

                    var messageId = GenerateMessageId(gelfServer);
                    for (var i = 0; i < chunkCount; i++)
                    {
                        var messageChunkPrefix = CreateChunkedMessagePart(messageId, i, chunkCount);
                        var skip = i * MaxChunkSize;
                        var messageChunkSuffix = gzipMessage.Skip(skip).Take(MaxChunkSize).ToArray();

                        var messageChunkFull = new byte[messageChunkPrefix.Length + messageChunkSuffix.Length];
                        messageChunkPrefix.CopyTo(messageChunkFull, 0);
                        messageChunkSuffix.CopyTo(messageChunkFull, messageChunkPrefix.Length);

                        udpClient.Send(messageChunkFull, messageChunkFull.Length);
                    }
                }
                else
                {
                    udpClient.Send(gzipMessage, gzipMessage.Length);
                }
            }
        }

        private static byte[] GzipMessage(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var ms = new MemoryStream();
            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;
            var compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);
            return compressed;
        }

        private static int GetGelfSeverity(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Error)
                return (int)GelfSeverity.Error;

            if (logLevel == LogLevel.Warn)
                return (int)GelfSeverity.Warning;
           
            if (logLevel == LogLevel.Info)
                return (int) GelfSeverity.Info;

            if (logLevel == LogLevel.Debug)
                return (int) GelfSeverity.Debug;

            if (logLevel == LogLevel.Fatal)
                return (int)GelfSeverity.Emergency;

            return (int) GelfSeverity.Trace;
        }

        private string CreateGelfJsonFromLoggingEvent(string body, Exception exception, LogLevel level)
        {
            var shortMessage = body.Length > ShortMessageLength ? body.Substring(0, ShortMessageLength - 1) : body;

            var gelfMessage = new GelfMessage
                {
                    Facility = Facility ?? "GELF",
                    FullMessage = body,
                    Host = Dns.GetHostName(),
                    Level = GetGelfSeverity(level),
                    ShortMessage = shortMessage,
                    TimeStamp = DateTime.Now,
                    Version = GelfVersion
                };

            if (exception == null) return JsonConvert.SerializeObject(gelfMessage);

            var exceptioToLog = exception;

            while (exceptioToLog.InnerException != null)
            {
                exceptioToLog = exceptioToLog.InnerException;
            }

            gelfMessage.ExceptionMessage = exceptioToLog.Message;
            gelfMessage.StackTrace = exceptioToLog.StackTrace;

            return JsonConvert.SerializeObject(gelfMessage);
        }

        private byte[] CreateChunkedMessagePart(string messageId, int chunkNumber, int chunkCount)
        {
            var result = new List<byte>
                {
                    Convert.ToByte(30),
                    Convert.ToByte(15)
                };

            //Chunked GELF ID: 0x1e 0x0f (identifying this message as a chunked GELF message)

            //Message ID: 32 bytes
            result.AddRange(Encoding.Default.GetBytes(messageId).ToArray());

            result.AddRange(GetChunkPart(chunkNumber, chunkCount));

            return result.ToArray<byte>();
        }

        private IEnumerable<byte> GetChunkPart(int chunkNumber, int chunkCount)
        {
            var bytes = new List<byte>();

            if (GraylogVersion != "0.9.6")
                bytes.Add(Convert.ToByte(0));

            bytes.Add(Convert.ToByte(chunkNumber));

            if (GraylogVersion != "0.9.6")
                bytes.Add(Convert.ToByte(0));

            bytes.Add(Convert.ToByte(chunkCount));

            return bytes.ToArray();
        }

        private string GenerateMessageId(string serverHostName)
        {
            var md5String = String.Join("", MD5.Create().ComputeHash(Encoding.Default.GetBytes(serverHostName)).Select(it => it.ToString("x2")).ToArray());
            var random = new Random((int) DateTime.Now.Ticks);
            var t = DateTime.Now.Ticks % 1000000000;
            var s = String.Format("{0}{1}", md5String.Substring(0, 10), md5String.Substring(20, 10));
            var r = random.Next(10000000).ToString("00000000");

            var sb = new StringBuilder(64);
            sb.Append(t);
            sb.Append(s);
            sb.Append(r);

            //Message ID: 32 bytes
            return sb.ToString().Substring(0, MaxHeaderSize);
        }

        #endregion

        #region Public Enums

        public enum GelfSeverity
        {
            Emergency = 0,
            Alert = 1,
            Critical = 2,
            Error = 3,
            Warning = 4,
            Trace = 5,
            Info = 6,
            Debug = 7
        };

        #endregion
    }
}
