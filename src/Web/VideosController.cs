using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using YoutubeRatings.Data;

namespace Web
{
    [Route("api/[controller]")]
    public class VideosController : Controller
    {
        private VideoRepository videos = new VideoRepository();
        private VideoViewCountRepository videoViewCounts = new VideoViewCountRepository();

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return videos.Get().Select(v => v.VideoId);
        }

        // GET api/values/5
        [HttpGet("{videoId}")]
        public string Get(string videoId)
        {
            return string.Join("\n", videoViewCounts.GetById(videoId).Select(vvc => $"{vvc.DateTime},{vvc.ViewCount}"));
        }

        [HttpGet("{videoId}/hourly")]
        public string Hourly(string videoId)
        {
            return string.Join("\n", videoViewCounts.GetById(videoId)
                .GroupBy(vvc => (vvc.DateTime.Date + new TimeSpan(vvc.DateTime.Hour, 0, 0)))
                .Select(vvcs => $"{vvcs.Key},{vvcs.Average(vvc => vvc.ViewCount)}"));
        }
    }
}
