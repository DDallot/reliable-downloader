using ReliableDownloader.Configurations;
using ReliableDownloader.Logic;
using ReliableDownloader.Validations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Reliable Downloader!");
            // If this url 404's, you can get a live one from https://installerstaging.accurx.com/chain/latest.json.
            var exampleUrl = "https://installerstaging.accurx.com/chain/3.55.11050.0/accuRx.Installer.Local.msi";
            var exampleFilePath = @"C:\Users\Diogo Dallot\Downloads\myfirstdownload.msi";

            // Bootstrapper
            var webCalls = new WebSystemCalls();
            var configuration = new Configuration();
            var getter = new Getter(webCalls);
            var writer = new Writer(webCalls);
            var lifeGuardGetter = new LifeGuardGetter(getter, configuration);
            var lifeGuardWriter = new LifeGuardWriter(writer, configuration);
            var validateChecksum = new ValidateChecksum();
            var cancellationTokenSource = new CancellationTokenSource();
            var fileDownloader = new FileDownloader(lifeGuardGetter, lifeGuardWriter, validateChecksum, cancellationTokenSource);

            bool result = false;
            do
            {
                Console.WriteLine("Starting Download...");
                var task = fileDownloader.DownloadFile(exampleUrl, exampleFilePath, progress => 
                { 
                    Console.WriteLine($"> Percent progress is {progress.ProgressPercent}% Estimated Remaining is {progress.EstimatedRemaining}"); 
                
                }, new TimeSpan(0, 0, 2));

                Console.WriteLine("Press <c> to stop download.");
                var key = Console.ReadKey(true).Key;
                while (key != ConsoleKey.C && !task.IsCompleted)
                {
                    key = Console.ReadKey(true).Key;
                } 

                if(key == ConsoleKey.C)
                {
                    Console.WriteLine("> c");
                    fileDownloader.CancelDownloads();
                }

                try
                {
                    result = await task;
                    if(!result) Console.WriteLine($"Download Failed");
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Task Canceled");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Exception:{ex}");
                }
            }while (!result);

            Console.WriteLine("Bye Reliable Downloader!");
        }
    }
}