using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/BannerSlides")]
    public class BannerSlideController : Controller
    {
        private IBannerSlideRepository _bannerRepo;

        public BannerSlideController(IBannerSlideRepository bannerRepo)
        {
            _bannerRepo = bannerRepo;
        }

        [HttpGet]
        public IActionResult GetBannerSlides()
        {
            return Ok(_bannerRepo.BannerSlides);
        }
    }
}