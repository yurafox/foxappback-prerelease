using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Infrastructure.Extensions;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IDictionaryRepository _dictRepo;

        public ProductController(IDictionaryRepository dictRepo) {
            _dictRepo = dictRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var products = _dictRepo.ProductsCache.WithOutEntityNavigation();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var prod =_dictRepo.ProductCache(id);
            var rez = OrmExtension.WithOutEntityNavigation(prod);

            return Ok(rez);
        }


        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var products = _dictRepo.ProductsCache
                .Where(p => p.PRODUCTS_IN_GROUPS.Any(gr => gr.ID_GROUP == categoryId))
                .WithOutEntityNavigation();

            return (!products.Any()) ? (IActionResult) BadRequest($"category doesn't contains products")
                                                        : Ok(products);

        }
    }
}