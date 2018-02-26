using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Cart")]
    public class CartController : Controller
    {
        private ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo) => _cartRepo = cartRepo;

        [HttpGet("CartProducts")]
        public IActionResult CartProducts() {
            var userID = 3; //TODO
            return Ok(_cartRepo.GetClientOrderProductsByUserId(userID));
        }

        [HttpPut("CartProducts")]
        public IActionResult UpdateCartProduct([FromBody] ClientOrderProduct_DTO item)
        {
            return Ok(_cartRepo.UpdateCartProduct(item));
        }

        [HttpPost("CartProducts")]
        public IActionResult CreateCartProduct([FromBody] ClientOrderProduct_DTO item)
        {
            ClientOrderProduct_DTO result = _cartRepo.InsertCartProduct(item);
            return CreatedAtRoute("", new {id = result.id}, result);
        }

        [HttpDelete("CartProducts/{id}")]
        public IActionResult DeleteCartProduct(long id)
        {
            _cartRepo.DeleteCartProduct(id);
            return NoContent();
        }


    }
}