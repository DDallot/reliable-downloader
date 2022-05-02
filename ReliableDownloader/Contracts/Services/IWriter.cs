using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Contracts.Services
{
    public interface IWriter
    {
        Task SavePartialContentAsync(string contentFileUrl, string localFilePath, long from, long to, CancellationToken token);
        Task SaveContentAsync(string contentFileUrl, string localFilePath, CancellationToken token);
    }
}
