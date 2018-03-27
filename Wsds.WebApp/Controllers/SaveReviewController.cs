using System.Linq;
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
    [Route("api/SaveReview")]
    public class SaveReviewController : Controller
    {
        private IClientRepository _cRepo;
        private IReviewRepository _rRepo;

        public SaveReviewController(IClientRepository cRepo, IReviewRepository rRepo)
        {
            _cRepo = cRepo;
            _rRepo = rRepo;
        }

        [Authorize]
        [HttpPost("Product")]
        [PullToken]
        public IActionResult SaveProductReview([FromBody] ProductReview_DTO review)
        {
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    if (review != null)
                    {
                        ProductReview_DTO result = _rRepo.SaveProductReview(review, client);
                        if (result != null)
                        {
                            return CreatedAtRoute("", result);
                        }
                    }
                }
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("Store")]
        [PullToken]
        public IActionResult SaveStoreReview([FromBody] StoreReview_DTO review)
        {
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    if (review != null)
                    {
                        StoreReview_DTO result = _rRepo.SaveStoreReview(review, client);
                        if (result != null)
                        {
                            return CreatedAtRoute("", result);
                        }
                    }
                }
            }
            return BadRequest();
        }
    }
}