namespace ReliableDownloader.Models
{
    public class FileHeader
    {
        public bool HasPartialLoad { get; set; }
        public bool HasError { get; set; }
        public long ContentLength { get; set; }
        public byte[] ContentMD5 { get; set; }
    }
}
