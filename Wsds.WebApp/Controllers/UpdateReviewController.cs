using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Entities;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/UpdateReview")]
    public class UpdateReviewController : Controller
    {
        private IClientRepository _cRepo;
        private IReviewRepository _rRepo;

        public UpdateReviewController(IClientRepository cRepo, IReviewRepository rRepo)
        {
            _cRepo = cRepo;
            _rRepo = rRepo;
        }

        [HttpPost("Product")]
        public IActionResult UpdateProductReview([FromBody] ProductReview_DTO review)
        {
            if (review != null)
            {
                ProductReview_DTO result = _rRepo.UpdateProductReview(review);
                if (result != null)
                {
                    return CreatedAtRoute("", result);
                }
            }
            return BadRequest();
        }
            

        [HttpPost("Store")]
        public IActionResult UpdateStoreReview([FromBody] StoreReview_DTO review)
        {
            if (review != null)
            {
                StoreReview_DTO result = _rRepo.UpdateStoreReview(review);
                if (result != null)
                {
                    return CreatedAtRoute("", result);
                }
            }
            return BadRequest();
        }
    }
}