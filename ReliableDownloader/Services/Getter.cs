using ReliableDownloader.Contracts.Services;
using ReliableDownloader.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Logic
{
    public class Getter : IGetter
    {
        private readonly IWebSystemCalls _webSystemCalls;

        public Getter(IWebSystemCalls webSystemCalls)
        {
            _webSystemCalls = webSystemCalls ?? throw new ArgumentNullException(nameof(webSystemCalls));
        }

        public async Task<FileHeader> GetHeadersAsync(string contentFileUrl, CancellationToken token)
        {
            var headersRes = await _webSystemCalls.GetHeadersAsync(contentFileUrl, token).ConfigureAwait(false);

            if (headersRes == null || !headersRes.IsSuccessStatusCode) return new FileHeader { HasError = true };

            var partialLoad = headersRes.Headers.AcceptRanges.Contains("bytes");
            var contentLength = headersRes.Content?.Headers?.ContentLength ?? 0;
            var contentMD5 = headersRes.Content?.Headers?.ContentMD5;

            return new FileHeader
            {
                HasPartialLoad = partialLoad,
                HasError = false,
                ContentLength = contentLength,
                ContentMD5 = contentMD5
            };
        }
    }
}
