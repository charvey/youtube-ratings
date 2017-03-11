using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            //return new[] { "" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }
    }
}
