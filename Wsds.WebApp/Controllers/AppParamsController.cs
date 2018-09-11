using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Models;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AppParamsController : Controller
    {

        private readonly IAppParamsRepository _repo;

        public AppParamsController(IAppParamsRepository repo) => _repo = repo;

        [HttpGet]
        public IActionResult Get() => Ok(_repo.GetAppParams());

        [HttpGet("health")]
        public IActionResult Health() => new ContentResult()
        {
            ContentType = "text/html; charset=utf-8",
            Content = HealthCheckModel.KeyPhrase
        };
    }
}