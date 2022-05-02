using ReliableDownloader.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Contracts.Services
{
    public interface IGetter
    {
        Task<FileHeader> GetHeadersAsync(string contentFileUrl, CancellationToken token);
    }
}
