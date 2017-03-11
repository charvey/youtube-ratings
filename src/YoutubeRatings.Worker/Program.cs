using Hangfire;
using Hangfire.MemoryStorage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

namespace YoutubeRatings.Worker
{
    public static class Program
    {
        private static VideoViewCountRepository viewCounts = new VideoViewCountRepository();

        public static void Main(string[] args)
        {
            var running = true;
            var loopSpeed = 6;

            GlobalConfiguration.Configuration.UseMemoryStorage();

            UsersProcess.Start();
            VideosProcess.Start();
            ViewsTracker.Start();

            using (var server = new BackgroundJobServer(new BackgroundJobServerOptions { WorkerCount = 20 }))
            {
                while (running)
                {
                    var output = new StringBuilder();
                    {
                        var api = JobStorage.Current.GetMonitoringApi();
                        var statistics = api.GetStatistics();
                        output.AppendLine($"{statistics.Scheduled} Scheduled");
                        output.AppendLine($"{statistics.Enqueued} Enqueued");
                        output.AppendLine($"{statistics.Processing} Processing");
                        output.AppendLine($"{statistics.Failed} Failed");
                        output.AppendLine($"{statistics.Succeeded} Succeeded");
                        output.AppendLine($"{statistics.Recurring} Recurring");
                    }

                    try
                    {
                        var api = JobStorage.Current.GetMonitoringApi();
                        var stat = viewCounts.Get().GroupBy(vvc => vvc.VideoId).OrderByDescending(ViewRate.CalculateGrowthPerHour);
                        output.AppendLine();
                        foreach (var x in stat.Take((Console.WindowHeight - 11) / 2))
                        {
                            var diff = x.Max(y => y.ViewCount) - x.Min(y => y.ViewCount);
                            output.AppendLine($"{x.Key}\t{x.Count(),5}\t{x.Last().ViewCount,10:F0}\t{1.0 * diff / x.Last().ViewCount,8:P}\t{ViewRate.CalculateViewsPerMinute(x),10:F3}\t{ViewRate.CalculateGrowthPerHour(x),8:P}");
                        }
                    }
                    catch
                    {
                        output.AppendLine("Problem reading top videos");
                    }

                    try
                    {
                        output.AppendLine();
                        output.AppendLine($"Currently {DateTime.UtcNow}");
                        var jobs = JobStorage.Current.GetMonitoringApi().ScheduledJobs(0, int.MaxValue);
                        foreach (var job in jobs.OrderBy(x => x.Value.EnqueueAt).Take((Console.WindowHeight - 11) / 2))
                            output.AppendLine($"{job.Value.Job.Method.Name} {string.Join(",", job.Value.Job.Args)} {job.Value.EnqueueAt}");
                    }
                    catch (KeyNotFoundException)
                    {
                        output.AppendLine("Problem reading ScheduledJobs");
                    }

                    output.AppendLine();
                    output.AppendLine($"Press 'x' to exit ({TimeSpan.FromSeconds(loopSpeed)})");

                    Console.Clear();
                    Console.Write(output);

                    Task.Factory.StartNew(() =>
                    {
                        var info = Console.ReadKey();
                        Console.WriteLine();
                        if (info.KeyChar == 'f')
                            loopSpeed = Math.Max(1, loopSpeed - 1);
                        else if (info.KeyChar == 's')
                            loopSpeed++;
                        else if (info.KeyChar == 'o')
                        {
                            var top = viewCounts.Get()
                                .GroupBy(vvc => vvc.VideoId)
                                .OrderByDescending(ViewRate.CalculateGrowthPerHour)
                                .First().Key;
                            Process.Start("chrome.exe", $"https://youtube.com/watch?v={top}");
                        }
                        else if (info.KeyChar == 'x')
                        {
                            running = false;
                            Console.WriteLine("Shutting down");
                        }
                        else
                            Console.WriteLine($"{info.KeyChar} doesn't mean anything");
                    }).Wait(TimeSpan.FromSeconds(loopSpeed));
                }
            }
        }
    }
}
