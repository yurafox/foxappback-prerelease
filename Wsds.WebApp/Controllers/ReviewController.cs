using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Reviews")]
    public class ReviewController : Controller
    {
        private IReviewRepository _revRepo;

        public ReviewController(IReviewRepository revRepo)
        {
            _revRepo = revRepo;
        }

        [HttpGet("GetProductReviews/{id}")]
        public IActionResult GetProductReviews(long id)
        {
            return Ok(_revRepo.GetProductReviews(id));
        }

        [HttpGet("GetStoreReviewsByStoreId/{id}")]
        public IActionResult GetStoreReviewsByStoreId(long id)
        {
            return Ok(_revRepo.GetStoreReviewsByStoreId(id));
        }

        [HttpGet("GetStoreReviews")]
        public IActionResult GetStoreReviews()
        {
            return Ok(_revRepo.GetStoreReviews());
        }
    }
}