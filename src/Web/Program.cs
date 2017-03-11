using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using YoutubeRatings.Worker;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();


            GlobalConfiguration.Configuration.UseMemoryStorage();

            UsersProcess.Start();
            VideosProcess.Start();
            ViewsTracker.Start();

            using (var server = new BackgroundJobServer(new BackgroundJobServerOptions { WorkerCount = 20 }))
                host.Run();
        }
    }
}
