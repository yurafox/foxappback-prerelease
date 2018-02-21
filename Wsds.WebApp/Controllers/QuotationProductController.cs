using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Attributes;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/QuotationProduct")]
    public class QuotationProductController : Controller
    {
        private readonly IQuotationProductRepository _repo;

        public QuotationProductController(IQuotationProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repo.QuotationProducts);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_repo.QuotationProduct(id));
        }

        
        [HttpGet]
        [Link("idProduct")]
        public IActionResult Get([FromQuery]string idProduct)
        {
            /*
            return Ok(
                        _repo.QuotationProducts.ToList()
                            .Where(x => (
                                            (x.idProduct == Int64.Parse(idProduct))
                                            && (x.stockQuant > 0)
                                         )
                                  )
                    ); */
            return Ok(_repo.GetQuotProdsByProductID(Int64.Parse(idProduct)));
        }


    }
}