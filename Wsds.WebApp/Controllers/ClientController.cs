using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Attributes;
using Newtonsoft.Json;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;

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

        [HttpGet]
        [Link("phone")]
        public IActionResult GetClientByPhone([FromQuery]string phone)
        {
            return Ok(_cliRepo.GetClientsByPhone(phone));
        }

        [HttpGet("person/{id}")]
        public IActionResult GetPersonById(long id) {
            return Ok(_cliRepo.GetPersonById(id));
        }

        [HttpPost("person")]
        public IActionResult CreatePerson([FromBody] PersonInfo_DTO person)
        {
            PersonInfo_DTO result = _cliRepo.CreatePerson(person);
            return CreatedAtRoute("", new { id = result.id }, result);
        }

        [HttpPut("person")]
        public IActionResult UpdatePerson([FromBody] PersonInfo_DTO person)
        {
            return Ok(_cliRepo.UpdatePerson(person));
        }

        [HttpGet("getBonusesInfo/{id}")]
        public IActionResult GetClientBonusesInfo(long id) {
            return Ok(_cliRepo.GetClientBonusesInfo(id));
        }

        [HttpGet("getBonusesExpireInfo")]
        [Link("clientId")]
        public IActionResult GetClientBonusesExpireInfo([FromQuery]long clientId)
        {
            return Ok( _cliRepo.GetClientBonusesExpireInfo(clientId));
        }

        [Authorize]
        [HttpPost("LogProductView")]
        [PullToken]
        public IActionResult CreateCartProduct([FromBody] LogProductViewRequest model)
        {
            var tModel = HttpContext.GetTokenModel();
            _cliRepo.LogProductView(model.idProduct, model.viewParams,tModel.ClientId);
            return Created("", null);
        }

        [HttpGet("ClientAddress/{id}")]
        public IActionResult ClientAddress(long id) {
            return Ok(_cliRepo.ClientAddress(id));
        }

        [HttpGet("ClientAddress")]
        [Link("idClient")]
        public IActionResult GetClientAddressesByClientId([FromQuery] long idClient) {
            return Ok(_cliRepo.GetClientAddressesByClientId(idClient));
        }

        [Authorize]
        [HttpPost("ClientAddress")]
        [PullToken]
        public IActionResult CreateClientAddress([FromBody] ClientAddress_DTO item)
        {
            var tModel = HttpContext.GetTokenModel();
            item.idClient = item.idClient ?? tModel.ClientId;

            ClientAddress_DTO result = _cliRepo.CreateClientAddress(item);
            return CreatedAtRoute("", new { id = result.id }, result);
        }

        [Authorize]
        [HttpPut("ClientAddress")]
        [PullToken]
        public IActionResult UpdateClientAddress([FromBody] ClientAddress_DTO item)
        {
            var tModel = HttpContext.GetTokenModel();
            item.idClient = item.idClient ?? tModel.ClientId;

            ClientAddress_DTO result = _cliRepo.UpdateClientAddress(item);
            return Ok(result);
        }

        [HttpDelete("ClientAddress/{id}")]
        public IActionResult DeleteClientAddress(long id) {
            _cliRepo.DeleteClientAddress(id);
            return NoContent();
        }
    }
}