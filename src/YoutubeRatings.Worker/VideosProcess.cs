using Hangfire;
using System;
using System.Linq;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

namespace YoutubeRatings.Worker
{
    public static class VideosProcess
    {
        private static VideoRepository videos = new VideoRepository();

        public static void Start()
        {
            RecurringJob.AddOrUpdate("Videos", () => ScheduleVideoUpdates(), Cron.MinuteInterval(5));
            RecurringJob.Trigger("Videos");
        }

        public static void ScheduleVideoUpdates()
        {
            videos.Get().ToList().ForEach(ScheduleVideoUpdate);
        }
        
        private static void ScheduleVideoUpdate(Video video)
        {
            var jobId = $"Update {video.VideoId}";
            var hash = Math.Abs(video.VideoId.GetHashCode());
            RecurringJob.AddOrUpdate(jobId, () => UpdateVideo(video.VideoId), Cron.Weekly((DayOfWeek)(hash % 7), hash % 24, hash % 60));
            if (string.IsNullOrEmpty(video.Title))
                RecurringJob.Trigger(jobId);
        }

        public static void UpdateVideo(string videoId)
        {
            videos.UpdateTitle(videoId, YouTubeApi.GetTitle(videoId));
        }
    }
}
