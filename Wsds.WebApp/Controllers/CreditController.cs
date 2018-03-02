using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Attributes;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Credit")]
    public class CreditController : Controller
    {
        private ICreditRepository _crRepo;

        public CreditController(ICreditRepository crRepo) => _crRepo = crRepo;

        [HttpGet("creditProduct")]
        public IActionResult GetCreditProducts() {
            return Ok(_crRepo.CreditProducts);
        }

        [HttpGet("GetProductCreditSize")]
        public IActionResult GetProductCreditSize([FromQuery]long idProduct, [FromQuery]long idSupplier)
        {
            return Ok(_crRepo.GetProductCreditSize(idProduct, idSupplier));
        }


    }
}