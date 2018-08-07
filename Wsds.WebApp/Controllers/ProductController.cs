using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Newtonsoft.Json;
using Wsds.WebApp.Attributes;
using Wsds.WebApp.WebExtensions;
using Microsoft.AspNetCore.Authorization;
using Wsds.DAL.Entities.Communication;
using Wsds.WebApp.Filters;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;

        public ProductController(IProductRepository prodRepo) {
            _prodRepo = prodRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_prodRepo.Products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id) => Ok(_prodRepo.Product(id));


        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(long categoryId)
        {
            return null;
            /*
            var products = _dictRepo.ProductsCache
                .Where(p => p.PRODUCTS_IN_GROUPS.Any(gr => gr.ID_GROUP == categoryId))
                .WithOutEntityNavigation();

            return (!products.Any()) ? (IActionResult) BadRequest($"category doesn't contains products")
                                                        : Ok(products);
                                                        */

        }
        [HttpGet ("GetProductDescription/{id}")]
        public IActionResult GetProductDescription(long id) {
            return Ok(new {description = _prodRepo.GetProductDescription(id)});
        }

        [HttpGet("GetProductImages/{id}")]
        public IActionResult GetProductImages(long id)
        {
            return Ok(new {images = _prodRepo.GetProductImages(id)});
        }

        [HttpGet]
        [Link("srch")]
        public IActionResult SearchProducts([FromQuery]string srch)
        {
            return Ok(_prodRepo.SearchProducts(srch));
        }

        [HttpGet("GetByAction/{actionId}")]
        public IActionResult GetByAction(long actionId)
        {
            var data = _prodRepo.GetByAction(actionId);
            return (data != null) ? Ok(data) : (IActionResult)BadRequest();
        }


        [HttpPost("NotifyOnProductArrival")]
        [PullToken(CanAnonymous = true)]
        public IActionResult NotifyOnProductArrival([FromBody] NotifyOnProductArrivalRequest request)
        {
            var tModel = HttpContext.GetTokenModel();
            _prodRepo.NotifyOnProductArrival(request, tModel.ClientId);
            return Created("", null);
        }
    }
}