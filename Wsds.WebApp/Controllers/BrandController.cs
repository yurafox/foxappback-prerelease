using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Infrastructure.Extensions;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BrandController : Controller
    {
        private readonly IBrandRepository _brandRepository;

        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpGet]
        public IActionResult GetBrands()
        {
            var brands = _brandRepository.GetAll
                           .WithOutEntityNavigation();

            return (brands != null)
                ? Ok(brands)
                : (IActionResult) BadRequest("can not get brands");
        }
        
        [HttpGet("{id}")]
        public IActionResult GetBrandById(int id)
        {
            var brand = _brandRepository.GetBrandById(id);
            return (brand != null)
                ? Ok(OrmExtension.WithOutEntityNavigation(brand))
                : (IActionResult)BadRequest($"can not get brand by id={id}");
        }

        [HttpGet("catalog/{categoryId}")]
        public IActionResult GetBrandByCategoryId(int categoryId)
        {
            var brands = _brandRepository.GetBrandByCategory(categoryId)
                .WithOutEntityNavigation();

            return (brands != null)
                ? Ok( brands)
                : (IActionResult)BadRequest($"can not get brands by category id={categoryId}");
        }
    }
}