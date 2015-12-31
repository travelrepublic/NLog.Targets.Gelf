using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace NLog.Targets.Gelf
{
    public static class MessageService
    {
        public static byte[] GzipMessage(string message)
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

        public static string CreateGelfJsonFromLoggingEvent(LogEventInfo logEventInfo, string facility, string host, 
            ICollection<GraylogParameterInfo> nlogParams, int maxShortLength = 250)
        {
            var shortMessage = logEventInfo.FormattedMessage.Length > maxShortLength ? logEventInfo.FormattedMessage.Substring(0, maxShortLength - 1) : logEventInfo.FormattedMessage;
            var messageBuilder = new GraylogMessageBuilder()
                .WithProperty("facility", facility)
                .WithProperty("short_message", shortMessage)
                .WithProperty("full_message", logEventInfo.FormattedMessage)
                .WithProperty("host", host)
                .WithLevel(logEventInfo.Level)
                .WithCustomProperty("logger", logEventInfo.LoggerName);


            if (logEventInfo.Properties != null)
            {
                object notes;
                if (logEventInfo.Properties.TryGetValue("Notes", out notes))
                    messageBuilder = messageBuilder.WithCustomProperty("notes", (string)notes);
            }

            if (nlogParams != null && nlogParams.Any())
            {
                var paramsDictionary = nlogParams
                    .Select(p => new KeyValuePair<string, string>(p.Name, p.Layout.Render(logEventInfo)))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                messageBuilder.WithCustomPropertyRange(paramsDictionary);
            }

            if (logEventInfo.Exception == null) return messageBuilder.Render();

            var exceptioToLog = logEventInfo.Exception;

            while (exceptioToLog.InnerException != null)
                exceptioToLog = exceptioToLog.InnerException;

            messageBuilder = messageBuilder.WithCustomProperty("exception_message", exceptioToLog.Message);
            messageBuilder = messageBuilder.WithCustomProperty("exception_stack_trace", exceptioToLog.StackTrace);

            return messageBuilder.Render();
        }
        
        public static byte[] CreateChunkedMessagePart(string messageId, int chunkNumber, int chunkCount)
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

        public static IEnumerable<byte> GetChunkPart(int chunkNumber, int chunkCount)
        {
            return new List<byte>
                {
                    Convert.ToByte(chunkNumber),
                    Convert.ToByte(chunkCount)
                };
        }

        public static string GenerateMessageId(int maxSize = 8)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var r = random.Next(10000000).ToString("00000000");

            //Message ID: 8 bytes
            return r.Substring(0, maxSize);
        }
    }
}
