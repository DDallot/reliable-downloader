using ReliableDownloader.Configurations;
using ReliableDownloader.Contracts.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Logic
{
    public class LifeGuardWriter : IWriter
    {
        private readonly IWriter _writer;
        private readonly Configuration _configuration;

        public LifeGuardWriter(IWriter writer, Configuration configuration)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SavePartialContentAsync(string contentFileUrl, string localFilePath, long from, long to, CancellationToken token)
        {
            do
            {
                try
                {
                    await _writer.SavePartialContentAsync(contentFileUrl, localFilePath, from, to, token);
                    return;
                }
                catch (Exception)
                {
                    await Task.Delay(_configuration.LifeGuardMillisecondsDelay, token);
                }
            } while (_configuration.LifeGuardEnable);
        }

        public async Task SaveContentAsync(string contentFileUrl, string localFilePath, CancellationToken token)
        {
            do
            {
                try
                {
                    await _writer.SaveContentAsync(contentFileUrl, localFilePath, token);
                    return;
                }
                catch (Exception)
                {
                    await Task.Delay(_configuration.LifeGuardMillisecondsDelay, token);
                }
            } while (_configuration.LifeGuardEnable);
        }
    }
}