using ReliableDownloader.Contracts.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Logic
{
    public class Writer : IWriter
    {
        private readonly IWebSystemCalls _webSystemCalls;

        public Writer(IWebSystemCalls webSystemCalls)
        {
            _webSystemCalls = webSystemCalls ?? throw new ArgumentNullException(nameof(webSystemCalls));
        }

        public async Task SavePartialContentAsync(string contentFileUrl, string localFilePath, long from, long to, CancellationToken token)
        {
            using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Write, FileShare.Write))
            {
                var message = await _webSystemCalls.DownloadPartialContent(contentFileUrl, from, to, token).ConfigureAwait(false);

                fileStream.Position = from;
                await message.Content.CopyToAsync(fileStream);
            }
        }

        public async Task SaveContentAsync(string contentFileUrl, string localFilePath, CancellationToken token)
        {
            using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var message = await _webSystemCalls.DownloadContent(contentFileUrl, token).ConfigureAwait(false);
                await message.Content.CopyToAsync(fileStream);
            }
        }
    }
}
