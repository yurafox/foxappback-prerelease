using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ActionController : Controller
    {
        private readonly IActionRepository _actionRepository;
        private readonly IProductRepository _prodRepo;

        public ActionController(IActionRepository actionRepository, IProductRepository prodRepo)
        {
            _actionRepository = actionRepository;
            _prodRepo = prodRepo;
        }

        [HttpGet]
        public IActionResult GetActions()
        {
            return Ok(_actionRepository.GetActions());
        }

        [HttpGet("{id}")]
        public IActionResult GetActionById(long id)
        {
            return Ok(_actionRepository.GetActionById(id));
        }

        [HttpGet("GetProductActions/{id}")]
        public IActionResult GetProductActions(long id)
        {
            return Ok(_actionRepository.GetProductActions(id));
        }

        [HttpGet("GetProductsOfDay")]
        public IActionResult GetProductsOfDay() => Ok(_actionRepository.GetProductsOfDay());

        [HttpGet("GetProductsSalesHits")]
        public IActionResult GetProductsSalesHits() => Ok(_actionRepository.GetProductsSalesHits());

    }
}