using System;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PageController : Controller
    {
        private readonly IPageRepository _pageRepository;

        public PageController(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetPageById(long id) => Ok(_pageRepository.GetPageById(id));

        [HttpGet("GetPageOptions/{id}")]
        public IActionResult GetPageOptions(long id)
        {
            var data = _pageRepository.GetPageOptions(id);
            return (!String.IsNullOrEmpty(data)) ? Ok(data) : (IActionResult)BadRequest();
        }
    }
}