using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        IProductGroupRepository _repo;

        public CatalogController(IProductGroupRepository repo)
        {

            _repo = repo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var data = _repo.ProductGroups;
            return (data != null && data.Count() != 0) 
                ? Ok(data)
                : (IActionResult) BadRequest("category list is empty");
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            var data = _repo.GetProductGroupById(id);
            return (data == null) ? (IActionResult)BadRequest($"can not find category by id={id}")
                                  : Ok(data);

        }
    }
}