using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
            var res = _cartRepo.UpdateCartProduct(item, tModel.ClientId, (long)tModel.SCN);
            Request.HttpContext.Response.Headers.Add("X-SCN", res.SCN.ToString());
            return Ok(res.Result);
        }

        [Authorize]
        [HttpPost("CartProducts")]
        [PullToken]
        public IActionResult CreateCartProduct([FromBody] ClientOrderProduct_DTO item)
        {
            var tModel = HttpContext.GetTokenModel();
            var res = _cartRepo.InsertCartProduct(item,tModel.ClientId,tModel.CurrencyId,tModel.IdApp,(long)tModel.SCN);
            Request.HttpContext.Response.Headers.Add("X-SCN", res.SCN.ToString());
            if (res.Result != null)
                return CreatedAtRoute("", new { id = res.Result.id }, res.Result);
            else
                return  StatusCode(409);
        }

        [Authorize]
        [HttpDelete("CartProducts/{id}")]
        [PullToken]
        public IActionResult DeleteCartProduct(long id)
        {
            var tModel = HttpContext.GetTokenModel();
            var res = _cartRepo.DeleteCartProduct(id,tModel.ClientId, (long)tModel.SCN);
            Request.HttpContext.Response.Headers.Add("X-SCN", res.SCN.ToString());
            return NoContent();
        }

        [Authorize]
        [HttpGet("ClientDraftOrder")]
        [PullToken]
        public IActionResult getClientDraftOrder()
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetOrCreateClientDraftOrder(tModel.ClientId,tModel.CurrencyId,tModel.IdApp));
        }

        [Authorize]
        [HttpGet("GetCartProductsByOrderId")]
        public IActionResult GetClientOrderProductsByOrderId([FromQuery] long idOrder)
        {
            return Ok(_cartRepo.GetClientOrderProductsByOrderId(idOrder));
        }

        [Authorize]
        [HttpGet("GetClientHistProductsByOrderId")]
        public IActionResult GetClientHistOrderProductsByOrderId([FromQuery] long idOrder)
        {
            return Ok(_cartRepo.GetClientHistOrderProductsByOrderId(idOrder));
        }


        [Authorize]
        [HttpGet("GetClientOrders")]
        [PullToken]
        public IActionResult GetClientOrders()
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetClientOrders(tModel.ClientId));
        }

        /*
        [Authorize]
        [HttpGet("ClientOrder/{id}")]
        [PullToken]
        public IActionResult GetClientOrders(long id)
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetClientOrder(id, tModel.ClientId));
        }
        */

        [Authorize]
        [HttpGet("ClientHistOrder/{id}")]
        [PullToken]
        public IActionResult GetClientHistOrder(long id)
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetClientHistOrder(id, tModel.ClientId));
        }
        

        [HttpPut("ClientDraftOrder")]
        [PullToken]
        public IActionResult SaveClientOrder([FromBody] ClientOrder_DTO order)
        {
            var tModel = HttpContext.GetTokenModel();
            var res = _cartRepo.SaveClientOrder(order, tModel.ClientId, (long)tModel.SCN);
            Request.HttpContext.Response.Headers.Add("X-SCN", res.SCN.ToString());
            return Ok(res.Result);
        }

        [Authorize]
        [HttpPost("CalculateCart")]
        [PullToken]
        public IActionResult CalculateCart([FromBody] CalculateCartRequest cart)
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.CalculateCart(cart, tModel.Card, tModel.ClientId, tModel.CurrencyId,tModel.IdApp));
        }

        [Authorize]
        [HttpPut("PostOrder")]
        [PullToken]
        public IActionResult PostOrder([FromBody] ClientOrder_DTO order)
        {   var tModel=HttpContext.GetTokenModel();
            order.idClient = order.idClient ?? tModel.ClientId;

            return Ok(_cartRepo.PostOrder(order));
        }

        [Authorize]
        [HttpGet("ClientOrderProductsByDate")]
        [PullToken]
        public IActionResult GetClientOrderProductsByDate([FromQuery] string datesRange)
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GetOrderProductsByDate(datesRange, tModel.ClientId));
        }

        [Authorize]
        [HttpPost("GenerateShipments")]
        [PullToken]
        public IActionResult GetGenerateShipments()
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.GenerateShipments(tModel.ClientId, tModel.CurrencyId,tModel.IdApp));
        }

        [Authorize]
        [HttpPut("shipment")]
        [PullToken]
        public IActionResult SaveShipment([FromBody] Shipment_DTO shipment)
        {
            var tModel = HttpContext.GetTokenModel();
            return Ok(_cartRepo.SaveShipment(shipment, tModel.ClientId));
        }
    }
}