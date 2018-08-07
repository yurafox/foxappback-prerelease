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
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FinController : Controller
    {
        IFinRepository _finRepo;

        public FinController(IFinRepository finRepo) {
            _finRepo = finRepo;
        }

        [HttpGet("pmtMethod")]
        public IActionResult GetPmtMethods() {
            return Ok(_finRepo.PaymentMethods);
        }

        [HttpGet("pmtMethod/{id}")]
        public IActionResult GetPmtMethodById(long id)
        {
            return Ok(_finRepo.PaymentMethod(id));
        }

    }   
}