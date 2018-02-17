using Hangfire;
using System.Linq;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

namespace YoutubeRatings.Worker
{
	public static class ChannelsProcess
	{
		private static VideoRepository videos = new VideoRepository();

		public static void Start()
		{
			RecurringJob.AddOrUpdate("Channels", () => ScheduleChannelUpdates(), Cron.MinuteInterval(10));
			RecurringJob.Trigger("Channels");
		}

		public static void ScheduleChannelUpdates()
		{
			new[] {
				YouTubeApi.GetChannelIdForUsername("CaseyNeistat"),
				YouTubeApi.GetChannelIdForUsername("GaryVaynerchuk"),
				YouTubeApi.GetChannelIdForUsername("KevinRose"),
				"UCmh5gdwCx6lN7gEC20leNVA" //DavidDobrik
			}.ToList().ForEach(SheduleChannelUpdate);
		}

		private static void SheduleChannelUpdate(string channelId)
		{
			RecurringJob.AddOrUpdate($"Update {channelId}", () => UpdateChannel(channelId), Cron.MinuteInterval(15));
		}

		public static void UpdateChannel(string channelId)
		{
			var playlistId = YouTubeApi.GetUploadsPlaylistId(channelId);
			var newItems = YouTubeApi.PlaylistItemsByPlaylistId(playlistId)
				.Except(videos.Get().Select(v => v.VideoId));
			foreach (var newItem in newItems)
				videos.Add(newItem);
		}
	}
}
