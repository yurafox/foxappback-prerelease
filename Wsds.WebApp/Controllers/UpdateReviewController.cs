using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;
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

        [Authorize]
        [HttpPost("Product")]
        [PullToken]
        public IActionResult UpdateProductReview([FromBody] ProductReview_DTO review)
        {
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    if (review != null)
                    {
                        ProductReview_DTO result = _rRepo.UpdateProductReview(review, client.id.Value);
                        if (result != null)
                        {
                            return CreatedAtRoute("", result);
                        }
                    }
                }
            }
            return Ok();
        }
            
        [Authorize]
        [HttpPost("Store")]
        [PullToken]
        public IActionResult UpdateStoreReview([FromBody] StoreReview_DTO review)
        {
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    if (review != null)
                    {
                        StoreReview_DTO result = _rRepo.UpdateStoreReview(review, client.id.Value);
                        if (result != null)
                        {
                            return CreatedAtRoute("", result);
                        }
                    }
                }
            }
            return Ok();
        }
    }
}