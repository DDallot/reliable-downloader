using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReliableDownloader.Contracts.Services;
using ReliableDownloader.Contracts.Validations;
using ReliableDownloader.Models;

namespace ReliableDownloader.Logic
{
    public class FileDownloader : IFileDownloader
    {
        private readonly IGetter _getter;
        private readonly IWriter _writer;
        private readonly IValidate _validate;
        private readonly double _chunks = 10;
        private CancellationTokenSource _cancellationTokenSource;

        public FileDownloader(IGetter getter, IWriter writer, IValidate validate, CancellationTokenSource cancellationTokenSource)
        {
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _validate = validate ?? throw new ArgumentNullException(nameof(validate));
            _cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(writer));
        }

        public async Task<bool> DownloadFile(string contentFileUrl, string localFilePath, Action<FileProgress> onProgressChanged, TimeSpan totalAverageTime)
        {
            var fileHeader = await _getter.GetHeadersAsync(contentFileUrl, _cancellationTokenSource.Token).ConfigureAwait(false);
            var fileProgress = new FileProgress(fileHeader.ContentLength, 0, 0, totalAverageTime, totalAverageTime);

            onProgressChanged(fileProgress);

            if (fileHeader.HasPartialLoad)
            {
                var chunkSize = (long)Math.Floor(fileHeader.ContentLength / _chunks);
                var tasks = new List<Task>();
                File.Create(localFilePath).Dispose();
                for (int i = 0; i < _chunks; ++i)
                {
                    var from = i * chunkSize;
                    var to = i + 1 != _chunks ? i * chunkSize + chunkSize : fileHeader.ContentLength;
                    tasks.Add(Task.Run(() => 
                        _writer.SavePartialContentAsync(contentFileUrl, localFilePath, from, to, _cancellationTokenSource.Token).ContinueWith(t => {
                            fileProgress.ReportProgress(to - from);
                            onProgressChanged(fileProgress);
                        })));
                }
                await Task.WhenAll(tasks);
            }
            else
            {
                await _writer.SaveContentAsync(contentFileUrl, localFilePath, _cancellationTokenSource.Token).ConfigureAwait(false);
                fileProgress.ReportProgress(fileHeader.ContentLength);
                onProgressChanged(fileProgress);
            }
            
            return _validate.IsValid(localFilePath, fileHeader);
        }

        public void CancelDownloads()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}