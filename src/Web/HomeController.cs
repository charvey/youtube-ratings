using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web
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

            return output.ToString();
        }
    }
}
