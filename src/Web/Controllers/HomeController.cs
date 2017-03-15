using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YoutubeRatings.Web.Controllers
{
    public class HomeController : Controller
    {
        private VideoViewCountRepository viewCounts = new VideoViewCountRepository();

        // GET: /<controller>/
        public string Index()
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
                output.AppendLine();
                foreach (var x in new TrendingVideos().Get().Take(10))
                {
                    output.AppendLine($"{x.Video.VideoId}\t{x.ViewCounts.Count(),5}\t{x.Last,10:F0}\t{x.Captured,8:P}\t{x.ViewsPerMinute,10:F3}\t{x.GrowthPerHour,8:P}");
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

            return output.ToString();
        }
    }
}
