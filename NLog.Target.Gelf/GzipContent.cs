using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NLog.Targets.Gelf
{
    internal sealed class GzipContent : HttpContent
    {
        private readonly HttpContent _content;

        public GzipContent(HttpContent content)
        {
            _content = content;

            // Keep the original content's headers ...
            foreach (var header in content.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // ... and let the server know we've Gzip-compressed the body of this request.
            Headers.ContentEncoding.Add("gzip");
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            // Open a GZipStream that writes to the specified output stream.
            using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
            {
                // Copy all the input content to the GZip stream.
                await _content.CopyToAsync(gzip);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
