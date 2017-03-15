using Microsoft.AspNetCore.Mvc;
using YoutubeRatings.Core;

namespace YoutubeRatings.Web.Controllers
{
    public class TrendingController : Controller
    {
        // GET: Trending
        public ActionResult Index()
        {
            return View(new TrendingVideos().Get());
        }
    }
}