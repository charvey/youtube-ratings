using Hangfire;
using System.Linq;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

namespace YoutubeRatings.Worker
{
    public static class UsersProcess
    {
        private static VideoRepository videos = new VideoRepository();

        public static void Start()
        {
            RecurringJob.AddOrUpdate("Users", () => ScheduleUserUpdates(), Cron.MinuteInterval(10));
            RecurringJob.Trigger("Users");
        }

        public static void ScheduleUserUpdates()
        {
            new[] { "CaseyNeistat", "GaryVaynerchuk", "KevinRose" }.ToList().ForEach(ScheduleUserUpdate);
        }

        private static void ScheduleUserUpdate(string username)
        {
            RecurringJob.AddOrUpdate($"Update {username}", () => UpdateUser(username), Cron.MinuteInterval(15));
        }

        public static void UpdateUser(string username)
        {
            var newItems = YouTubeApi.PlaylistItems(username)
                .Except(videos.Get().Select(v => v.VideoId));
            foreach (var newItem in newItems)
                videos.Add(newItem);
        }
    }
}
