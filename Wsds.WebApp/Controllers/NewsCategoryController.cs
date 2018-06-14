using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/NewsCategory")]
    public class NewsCategoryController : Controller
    {
        private INewsCategoryRepository _newsRepo;

        public NewsCategoryController(INewsCategoryRepository newsRepo)
        {
            _newsRepo = newsRepo;
        }

        [HttpGet]
        public IActionResult GetNewsCategory()
        {
            return Ok(_newsRepo.NewsCategory);
        }
    }
}