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
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _repo;

        public SupplierController(ISupplierRepository repo) => _repo = repo;

        [HttpGet]
        public IActionResult Get() => Ok(_repo.Suppliers);

        [HttpGet("{id}")]
        public IActionResult Get(long id) => Ok(_repo.Supplier(id));

    }
}