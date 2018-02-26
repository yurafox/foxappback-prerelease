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
    [Route("api/Client")]
    public class ClientController : Controller
    {
        private IClientRepository _cliRepo;

        public ClientController(IClientRepository cliRepo) => _cliRepo = cliRepo;

        [HttpGet]
        public IActionResult GetClients() => Ok(_cliRepo.Clients);

        [HttpGet("{id}")]
        public IActionResult GetClientById(long id) => Ok(_cliRepo.Client(id));

        [HttpGet]
        [Link("userId")]
        public IActionResult GetClientByUserID([FromQuery]string userId) {
            return Ok(_cliRepo.GetClientByUserID(Int64.Parse(userId)));
        }

        [HttpGet]
        [Link("email")]
        public IActionResult GetClientByEmail([FromQuery]string email)
        {
            return Ok(_cliRepo.GetClientByEmail(email));
        }

    }
}