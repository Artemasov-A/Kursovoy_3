using FileMonitor.BackgroundService;

namespace FIleMonitor.BackgroundService
{
    public class Program
    {
        static void Main(string[] args)
        {
            var backgroundJob = new BackgroundJob();

            backgroundJob.Execute();
        }
    }
}
