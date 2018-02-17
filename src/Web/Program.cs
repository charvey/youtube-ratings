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

            ChannelsProcess.Start();
            VideosProcess.Start();
            ViewsTracker.Start();

            host.Run();
        }
    }
}
