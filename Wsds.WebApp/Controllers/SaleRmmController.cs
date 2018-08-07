using Microsoft.AspNetCore.Mvc;
using System;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{

    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SaleRmmController : Controller
    {
        private ISaleRmmRepository _saleRmmRepo;
        private ICartRepository _cartRepo;

        public SaleRmmController(ISaleRmmRepository saleRmmRepo, ICartRepository cartRepo)
        {
            _saleRmmRepo = saleRmmRepo;
            _cartRepo = cartRepo;
        }

        
        [HttpPost("NewSaleRmm")]        
        public IActionResult PostNewSaleRmm([FromBody] ClientOrderMQ order)
        {
            if (order == null)
            {
                throw new ArgumentException("ClientOrderMQ cann't be null");
            }
            _saleRmmRepo.CreateSaleRmm(order);
            return Ok();
        }
    }
}
