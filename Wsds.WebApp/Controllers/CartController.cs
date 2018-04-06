using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Cart")]
    public class CartController : Controller
    {
        private ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo) => _cartRepo = cartRepo;

        [Authorize]
        [HttpGet("CartProducts")]
        [PullToken]
        public IActionResult CartProducts()
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetClientOrderProductsByClietId(tModel.ClientId));
        }

        [Authorize]
        [HttpPut("CartProducts")]
        [PullToken]
        public IActionResult UpdateCartProduct([FromBody] ClientOrderProduct_DTO item)
        {
            var tModel = HttpContext.GetTokenModel(); 
            return Ok(_cartRepo.UpdateCartProduct(item,tModel.ClientId));
        }

        [Authorize]
        [HttpPost("CartProducts")]
        [PullToken]
        public IActionResult CreateCartProduct([FromBody] ClientOrderProduct_DTO item)
        {
            var tModel = HttpContext.GetTokenModel();
            ClientOrderProduct_DTO result = _cartRepo.InsertCartProduct(item,tModel.ClientId,tModel.CurrencyId);
            return CreatedAtRoute("", new { id = result.id }, result);
        }

        [Authorize]
        [HttpDelete("CartProducts/{id}")]
        [PullToken]
        public IActionResult DeleteCartProduct(long id)
        {
            var tModel = HttpContext.GetTokenModel();
            _cartRepo.DeleteCartProduct(id,tModel.ClientId);
            return NoContent();
        }

        [Authorize]
        [HttpGet("ClientDraftOrder")]
        [PullToken]
        public IActionResult getClientDraftOrder()
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetOrCreateClientDraftOrder(tModel.ClientId,tModel.CurrencyId));
        }

        [HttpGet("GetCartProductsByOrderId")]
        public IActionResult GetClientOrderProductsByOrderId([FromQuery] long idOrder)
        {
            return Ok(_cartRepo.GetClientOrderProductsByOrderId(idOrder));
        }

        [Authorize]
        [HttpGet("GetClientOrders")]
        [PullToken]
        public IActionResult GetClientOrders()
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetClientOrders(tModel.ClientId));
        }

        [Authorize]
        [HttpPut("ClientDraftOrder")]
        [PullToken]
        public IActionResult SaveClientOrder([FromBody] ClientOrder_DTO order)
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.SaveClientOrder(order,tModel.ClientId));
        }

        [Authorize]
        [HttpPost("CalculateCart")]
        [PullToken]
        public IActionResult CalculateCart([FromBody] CalculateCartRequest cart)
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.CalculateCart(cart,tModel.Card));
        }

        [Authorize]
        [HttpPut("PostOrder")]
        [PullToken]
        public IActionResult PostOrder([FromBody] ClientOrder_DTO order)
        {   var tModel=HttpContext.GetTokenModel();
            order.idClient = order.idClient ?? tModel.ClientId;

            return Ok(_cartRepo.PostOrder(order));
        }
    }
}