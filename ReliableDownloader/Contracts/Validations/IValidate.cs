using ReliableDownloader.Models;

namespace ReliableDownloader.Contracts.Validations
{
    public interface IValidate
    {
        bool IsValid(string localFilePath, FileHeader fileHeader);
    }
}
