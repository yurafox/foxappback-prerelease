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
    public class NoveltyController : Controller
    {
        private readonly INoveltyRepository _noveltyRepository;

        public NoveltyController(INoveltyRepository noveltyRepository)
        {
            _noveltyRepository = noveltyRepository;
        }

        [HttpGet("GetNovelties")]
        public IActionResult GetNovelties()
        {
            return Ok(_noveltyRepository.GetNovelties());
        }

        [HttpGet("GetNoveltyById/{id}")]
        public IActionResult GetNoveltyById(long id)
        {
            return Ok(_noveltyRepository.GetNoveltyById(id));
        }

        [HttpGet("GetNoveltyDetailsByNoveltyId/{id}")]
        public IActionResult GetNoveltyDetailsByNoveltyId(long id)
        {
            return Ok(_noveltyRepository.GetNoveltyDetailsByNoveltyId(id));
        }
    }
}