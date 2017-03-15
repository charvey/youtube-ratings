using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeRatings.Data;

namespace YoutubeRatings.Core
{
    public class TrendingVideo
    {
        public Video Video { get; set; }
        public IEnumerable<VideoViewCount> ViewCounts { get; set; }
        public IEnumerable<VideoViewCount> RecentViewCounts { get; set; }

        public long First => ViewCounts.First().ViewCount;
        public long Last => ViewCounts.Last().ViewCount;
        public double Captured => 1.0 * (Last - First) / Last;
        public double GrowthPerHour => ViewRate.CalculateGrowthPerHour(RecentViewCounts);
        public double ViewsPerMinute => ViewRate.CalculateViewsPerMinute(RecentViewCounts);
    }

    public class TrendingVideos
    {
        private readonly VideoRepository videos = new VideoRepository();
        private readonly VideoViewCountRepository videoViewCounts = new VideoViewCountRepository();

        public IEnumerable<TrendingVideo> Get()
        {
            return videoViewCounts.Get()
                .GroupBy(x => x.VideoId)
               .Select(x => new TrendingVideo
               {
                   Video = videos.GetById(x.Key),
                   ViewCounts = x.AsEnumerable(),
                   RecentViewCounts = ViewsToBeConsidered(x)
               })
               .Where(x => x.RecentViewCounts.Count() > 1)
               .OrderByDescending(x => ViewRate.CalculateGrowthPerHour(x.RecentViewCounts));
        }

        private IEnumerable<VideoViewCount> ViewsToBeConsidered(IEnumerable<VideoViewCount> vvcs)
        {
            return vvcs.Where(vvc => DateTime.Now - vvc.DateTime <= TimeSpan.FromDays(1));
        }
    }
}
