using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using YoutubeRatings.Core;
using YoutubeRatings.Data;

namespace YoutubeRatings.Web.Controllers
{
    public class Trending
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public IEnumerable<VideoViewCount> ViewCounts { get; set; }
    }

    public class TrendingController : Controller
    {
        private VideoRepository videos = new VideoRepository();
        private VideoViewCountRepository viewCounts = new VideoViewCountRepository();

        // GET: Trending
        public ActionResult Index()
        {
            var stat = viewCounts.Get().GroupBy(vvc => vvc.VideoId)
                .OrderByDescending(ViewRate.CalculateGrowthPerHour)
                .Select(vvcs => new Trending
                {
                    VideoId = vvcs.Key,
                    Title = videos.GetById(vvcs.Key).Title,
                    ViewCounts = vvcs.AsEnumerable()
                });
            return View(stat);
        }
    }
}