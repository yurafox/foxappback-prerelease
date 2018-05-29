using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Reviews")]
    public class ReviewController : Controller
    {
        private IClientRepository _cRepo;
        private IReviewRepository _revRepo;

        public ReviewController(IReviewRepository revRepo, IClientRepository cRepo)
        {
            _revRepo = revRepo;
            _cRepo = cRepo;
        }

       
        [HttpGet("GetProductReviews/{id}")]
        [PullToken(CanAnonymous = true)]
        public IActionResult GetProductReviews(long id)
        {
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null)
            {
                var data = new {
                    productReviews = _revRepo.GetProductReviews(id, tokenModel.ClientId),
                    currentUser = tokenModel.ClientId
                };
                return Ok(data);
                
            }
            return Ok();
        }

        [HttpGet("GetStoreReviewsByStoreId/{id}")]
        [PullToken(CanAnonymous = true)]
        public IActionResult GetStoreReviewsByStoreId(long id)
        {
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null)
            {
                var data = new
                {
                    storeReviews = _revRepo.GetStoreReviewsByStoreId(id, tokenModel.ClientId),
                    currentUser = tokenModel.ClientId
                };
                return Ok(data);

            }
            return Ok();
        }

        [HttpGet("GetStoreReviews")]
        [PullToken(CanAnonymous = true)]
        public IActionResult GetStoreReviews()
        {
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null)
            {
                var data = new
                {
                    storeReviews = _revRepo.GetStoreReviews(tokenModel.ClientId),
                    currentUser = tokenModel.ClientId
                };
                return Ok(data);

            }
            return Ok();
        }
    }
}