using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Stock")]
    public class StockController : Controller
    {
        private readonly IActionRepository _actionRepository;

        public StockController(IActionRepository actionRepository)
        {
            _actionRepository = actionRepository;
        }

        [HttpGet]
        public IActionResult GetActions(IActionRepository actionRepository)
        {
            return Ok(_actionRepository.GetActions());
        }

        [HttpGet("{id}")]
        public IActionResult GetActionById(long id)
        {
            return Ok(_actionRepository.GetActionById(id));
        }
    }
}