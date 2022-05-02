using System;

namespace ReliableDownloader.Models
{
    public class FileProgress
    {
        public FileProgress(long? totalFileSize, long totalBytesDownloaded, double? progressPercent, TimeSpan? estimatedRemaining, TimeSpan totalAverageTime)
        {
            TotalFileSize = totalFileSize;
            TotalBytesDownloaded = totalBytesDownloaded;
            ProgressPercent = progressPercent;
            EstimatedRemaining = estimatedRemaining;
            TotalAverageTime = totalAverageTime;
            Started = DateTime.UtcNow;
        }

        public long? TotalFileSize { get; }
        public long TotalBytesDownloaded { get; private set;}
        public double? ProgressPercent { get; private set; }
        public TimeSpan? EstimatedRemaining { get; private set; }
        public TimeSpan TotalAverageTime { get; }
        public DateTime Started { get; }

        public void ReportProgress(long bytesDownloaded)
        {
            if (bytesDownloaded == 0) return;

            lock (this)
            {
                TotalBytesDownloaded += bytesDownloaded;
                ProgressPercent = TotalBytesDownloaded * 100 / TotalFileSize;
                if(ProgressPercent >= 50)
                {
                    var interval = DateTime.UtcNow - Started;
                    EstimatedRemaining = (100 - ProgressPercent) * interval / ProgressPercent;
                }
                else
                {
                    EstimatedRemaining = TotalAverageTime * (100 - ProgressPercent) / 100;
                }
            }
        }
    }
}