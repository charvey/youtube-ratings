using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace YoutubeRatings.Core
{
    public static class VideoContentDetailsExtension
    {
        public static TimeSpan DurationTimeSpan(this VideoContentDetails vcd)
        {
            return XmlConvert.ToTimeSpan(vcd.Duration);
        }
    }

	public static class YouTubeApi
	{
        private static YouTubeService service = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = ConfigurationManager.AppSettings["youtube-api-key"],
            ApplicationName = "youtube-rating"
        });

		public static TimeSpan GetDuration(string videoId)
		{
			var request = service.Videos.List("contentDetails");
			request.Id = videoId;
			var videoListResponse = request.Execute();
			return videoListResponse.Items.Single().ContentDetails.DurationTimeSpan();
		}

        public static string GetTitle(string videoId)
        {
            var request = service.Videos.List("snippet");
            request.Id = videoId;
            var videoListResponse = request.Execute();
            return videoListResponse.Items.Single().Snippet.Title;
        }

        public static ulong GetViewCount(string videoId)
		{
			var request = service.Videos.List("statistics");
			request.Id = videoId;
			var videoListResponse = request.Execute();
			return videoListResponse.Items.Single().Statistics.ViewCount.Value;
		}

		public static string GetUploadsPlaylistId(string username)
		{
			var channel = service.Channels.List("contentDetails");
			channel.ForUsername = username;
			return channel.Execute().Items.Single().ContentDetails.RelatedPlaylists.Uploads;
		}

		public static IList<string> PlaylistItems(string username)
		{
            return PlaylistItemsByPlaylistId(GetUploadsPlaylistId(username)).ToList();
		}

		public static IEnumerable<string> PlaylistItemsByPlaylistId(string playlistId)
		{
			var playlistItems = service.PlaylistItems.List("contentDetails");
			playlistItems.PlaylistId = playlistId;
			return PlaylistItems(playlistItems).Select(pi => pi.ContentDetails.VideoId);
		}

		private static IEnumerable<PlaylistItem> PlaylistItems(PlaylistItemsResource.ListRequest request)
		{
			request.MaxResults = 50;
			while (true)
			{
				var response = request.Execute();

				foreach (var item in response.Items)
					yield return item;

				if (response.NextPageToken == null)
					yield break;

				request.PageToken = response.NextPageToken;
			}
		}
	}
}
