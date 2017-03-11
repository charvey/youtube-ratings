using System.Collections.Generic;
using System.Linq;
using YoutubeRatings.Data;

namespace YoutubeRatings.Core
{
	public static class ViewRate
	{
		public static double CalculateViewsPerMinute(IEnumerable<VideoViewCount> viewCounts)
		{
			var recentViewCounts = viewCounts.OrderByDescending(x => x.DateTime).Take(2);
			var viewDifference = recentViewCounts.First().ViewCount - recentViewCounts.Last().ViewCount;
			var timeDifference = recentViewCounts.First().DateTime - recentViewCounts.Last().DateTime;
			return viewDifference / timeDifference.TotalMinutes;
		}

		public static double CalculateGrowthPerHour(IEnumerable<VideoViewCount> viewCounts)
		{
			var recentViewCounts = viewCounts.OrderByDescending(x => x.DateTime).Take(2);
			var viewDifference = recentViewCounts.First().ViewCount - recentViewCounts.Last().ViewCount;
			var growth = (1.0 * viewDifference) / ((recentViewCounts.First().ViewCount + recentViewCounts.Last().ViewCount) / 2);
			var timeDifference = recentViewCounts.First().DateTime - recentViewCounts.Last().DateTime;
			return growth / timeDifference.TotalHours;
		}

		public static IEnumerable<KeyValuePair<string, double>> MostViewsPerMinute(IEnumerable<VideoViewCount> viewCounts)
		{
			return viewCounts.GroupBy(v => v.VideoId)
				.Select(g => new KeyValuePair<string, double>(g.Key, CalculateViewsPerMinute(g)))
				.OrderByDescending(g => g.Value);
		}
	}
}
