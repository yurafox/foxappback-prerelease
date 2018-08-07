using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ManufacturerController : Controller
    {
        private readonly IManufacturerRepository _dictRepo;

        public ManufacturerController(IManufacturerRepository dictRepo) => _dictRepo = dictRepo;

        [HttpGet]
        public IActionResult Get() => Ok(_dictRepo.Manufacturers);

        [HttpGet("{id}")]
        public IActionResult Get(long id) => Ok(_dictRepo.Manufacturer(id));

        [HttpGet("catalog/{categoryId}")]
        public IActionResult GetBrandByCategoryId(long categoryId)
        {
            return null;
            /*
            var brands = _brandRepository.GetBrandByCategory(categoryId)
                .WithOutEntityNavigation();

            return (brands != null)
                ? Ok( brands)
                : (IActionResult)BadRequest($"can not get brands by category id={categoryId}");
                */
        }
    }
}