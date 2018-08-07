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
    public class LocalizationController : Controller
    {
        private readonly ILocalizationRepository _locRepo;

        public LocalizationController(ILocalizationRepository locRepo) => _locRepo = locRepo;

        [HttpGet("lang")]
        public IActionResult Get() => Ok(_locRepo.Langs);

        [HttpGet("lang/{id}")]
        public IActionResult Get(long id) => Ok(_locRepo.Lang(id));
    }
}