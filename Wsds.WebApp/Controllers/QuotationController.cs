using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Quotation")]
    public class QuotationController : Controller
    {
        private IQuotationRepository _quotRepo;

        public QuotationController(IQuotationRepository quotRepo) => _quotRepo = quotRepo;

        [HttpGet]
        public IActionResult GetQuotes() => Ok(_quotRepo.Quotations);

        [HttpGet("{id}")]
        public IActionResult GetQouteById(long id) => Ok(_quotRepo.Quotation(id));
    }
}