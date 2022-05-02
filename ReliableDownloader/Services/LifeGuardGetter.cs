using ReliableDownloader.Configurations;
using ReliableDownloader.Contracts.Services;
using ReliableDownloader.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Logic
{
    public class LifeGuardGetter : IGetter
    {
        private readonly IGetter _getter;
        private readonly Configuration _configuration;

        public LifeGuardGetter(IGetter getter, Configuration configuration)
        {
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<FileHeader> GetHeadersAsync(string contentFileUrl, CancellationToken token)
        {
            do
            {
                try
                {
                    return await _getter.GetHeadersAsync(contentFileUrl, token);
                }
                catch (Exception)
                {
                    await Task.Delay(_configuration.LifeGuardMillisecondsDelay, token);
                }
            } while (_configuration.LifeGuardEnable);

            return new FileHeader { HasError = true};
        }
    }
}
