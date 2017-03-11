using Hangfire;
using System;
using System.Collections.Concurrent;
using System.Linq;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

namespace YoutubeRatings.Worker
{
    public static class ViewsTracker
    {
        private static ConcurrentDictionary<string, string> jobs = new ConcurrentDictionary<string, string>();
        private static VideoRepository videos = new VideoRepository();
        private static VideoViewCountRepository viewCounts = new VideoViewCountRepository();

        public static void Start()
        {
            RecurringJob.AddOrUpdate("ViewCounts", () => UpdateVideoViewCounts(), Cron.Minutely);
            RecurringJob.Trigger("ViewCounts");
        }

        public static void UpdateVideoViewCounts()
        {
            videos.Get().Select(v => v.VideoId).ToList().ForEach(ScheduleUpdateVideoViewCount);
        }

        private static void ScheduleUpdateVideoViewCount(string videoId)
        {
            if (jobs.ContainsKey(videoId)) return;

            DateTime goal = DateTime.Now;

            var recent = viewCounts.GetById(videoId).OrderByDescending(x => x.DateTime).ToList();
            if (!recent.Any())
            {
                goal = DateTime.Now;
            }
            else if (recent.Count() < 5)
            {
                goal = DateTime.Now;
            }
            else
            {
                var diff = (recent.First().ViewCount - recent.ElementAt(4).ViewCount);
                var timeDiff = recent.First().DateTime - recent.ElementAt(4).DateTime;
                var avg = (recent.First().ViewCount + recent.ElementAt(4).ViewCount) / 2.0;
                var n = diff / (0.0005 * avg);
                var timeInterval = timeDiff.TotalMinutes / n;

                if (double.IsInfinity(timeInterval))
                    goal = DateTime.MaxValue;
                else
                    goal = recent.First().DateTime + TimeSpan.FromMinutes(timeInterval);
            }

            if (recent.Any())
            {
                if (DateTime.Now - recent.Last().DateTime < TimeSpan.FromHours(1))
                {
                    var newGoal = recent.First().DateTime + TimeSpan.FromMinutes(5);
                    if (newGoal < goal) goal = newGoal;
                }
                else if (DateTime.Now - recent.Last().DateTime < TimeSpan.FromHours(12))
                {
                    var newGoal = recent.First().DateTime + TimeSpan.FromMinutes(15);
                    if (newGoal < goal) goal = newGoal;
                }
                else if (DateTime.Now - recent.Last().DateTime < TimeSpan.FromDays(1))
                {
                    var newGoal = recent.First().DateTime + TimeSpan.FromHours(1);
                    if (newGoal < goal) goal = newGoal;
                }
                else
                {
                    var newGoal = recent.First().DateTime + TimeSpan.FromDays(0.5);
                    if (newGoal < goal) goal = newGoal;
                }
            }

            jobs.AddOrUpdate(videoId,
                (k) => BackgroundJob.Schedule(() => UpdateVideoViewCount(k), goal),
                (k, v) =>
                {
                    BackgroundJob.Delete(v);
                    return BackgroundJob.Schedule(() => UpdateVideoViewCount(k), goal);
                });
        }

        public static void UpdateVideoViewCount(string videoId)
        {
            viewCounts.Add(new VideoViewCount
            {
                DateTime = DateTime.Now,
                VideoId = videoId,
                ViewCount = (long)YouTubeApi.GetViewCount(videoId)
            });
            jobs.TryRemove(videoId, out string jobId);
            ScheduleUpdateVideoViewCount(videoId);
        }
    }
}
