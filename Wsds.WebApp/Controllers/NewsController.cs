using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class NewsController : Controller
    {
        private INewsRepository _newsRepo;

        public NewsController(INewsRepository newsRepo)
        {
            _newsRepo = newsRepo;
        }

        [HttpGet]
        public IActionResult GetNews()
        {
            return Ok(_newsRepo.News);
        }

        [HttpGet("GetNewsDescription/{id}")]
        public IActionResult GetNewsDescription(long id)
        {
            return Ok(new { description = _newsRepo.GetNewsDescription(id) });
        }

        [HttpGet("GetNewsByCategory/{categoryId}")]
        public IActionResult GetNewsByCategory(int categoryId)
        {
            return Ok(_newsRepo.SearchNewsInCache(categoryId));
        }
    }
}