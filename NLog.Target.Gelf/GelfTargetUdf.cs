using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NLog.Config;

namespace NLog.Targets.Gelf
{
    [Target("Gelf")]
    public sealed class GelfTargetUdf : Target
    {
        #region Private Members

        private static readonly Socket SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static readonly Socket SocketClientV6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
        private static readonly ConcurrentDictionary<string, IPEndPoint> EndPoints = new ConcurrentDictionary<string, IPEndPoint>();
        private static readonly string Host = Dns.GetHostName();

        private const int MaxNumberOfChunksAllowed = 128;

        #endregion

        #region Public properties

        [RequiredParameter]
        public string GelfServer { get; set; }

        public int Port { get; set; }
        public string Facility { get; set; }
        public int MaxChunkSize { get; set; }

        [ArrayParameter(typeof(GraylogParameterInfo), "parameter")]
        public IList<GraylogParameterInfo> Parameters { get; private set; }

        #endregion

        #region Public Constructors

        public GelfTargetUdf()
        {
            GelfServer = "127.0.0.1";
            Port = 12201;
            Facility = null;
            MaxChunkSize = 1024;
            Parameters = new List<GraylogParameterInfo>();
        }

        #endregion

        #region Overridden NLog methods

        protected override void InitializeTarget()
        {
            if (Facility == null) Facility = ConfigurationManager.AppSettings["AppName"];

            base.InitializeTarget();
        }

        /// <summary>
        /// This is where we hook into NLog, by overriding the Write method. 
        /// </summary>
        /// <param name="logEvent">The NLog.LogEventInfo </param>
        protected override void Write(LogEventInfo logEvent)
        {
            SendMessage(MessageService.CreateGelfJsonFromLoggingEvent(logEvent, Facility, Host, Parameters));
        }

        #endregion

        #region Private Methods

        private void SendMessage(string message)
        {
            var endPoint = GetIPEndPoint(GelfServer, Port);
            var thisSock = endPoint.AddressFamily == AddressFamily.InterNetworkV6 ? SocketClientV6 : SocketClient;

            var gzipMessage = MessageService.GzipMessage(message);
            if (gzipMessage.Length > MaxChunkSize)
            {
                var chunkCount = (gzipMessage.Length / MaxChunkSize) + 1;
                if (chunkCount > MaxNumberOfChunksAllowed)
                    return;

                var messageId = MessageService.GenerateMessageId();
                for (var i = 0; i < chunkCount; i++)
                {
                    var messageChunkPrefix = MessageService.CreateChunkedMessagePart(messageId, i, chunkCount);
                    var skip = i * MaxChunkSize;
                    var messageChunkSuffix = gzipMessage.Skip(skip).Take(MaxChunkSize).ToArray();

                    var messageChunkFull = new byte[messageChunkPrefix.Length + messageChunkSuffix.Length];
                    messageChunkPrefix.CopyTo(messageChunkFull, 0);
                    messageChunkSuffix.CopyTo(messageChunkFull, messageChunkPrefix.Length);

                    thisSock.SendTo(messageChunkFull, 0, messageChunkFull.Length, SocketFlags.None, endPoint);
                }
            }
            else
            {
                thisSock.SendTo(gzipMessage, 0, gzipMessage.Length, SocketFlags.None, endPoint);
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
        

        #endregion
    }
}
