using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;
using Wsds.DAL.Entities.DTO;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/ClientMessage")]
    public class ClientMessageController : Controller
    {
        private IClientRepository _cRepo;
        private IClientMessageRepository _cmRepo;

        public ClientMessageController(IClientRepository cRepo, IClientMessageRepository cmRepo)
        {
            _cRepo = cRepo;
            _cmRepo = cmRepo;
        }

        [Authorize]
        [HttpPost]
        [PullToken]
        public IActionResult SaveClientMessage([FromBody] ClientMessage_DTO message)
        {
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    if (message != null)
                    {
                        ClientMessage_DTO result = _cmRepo.SaveClientMessage(message, (long)client.id);
                        if (result != null)
                        {
                            return CreatedAtRoute("", result);
                        }
                    }
                }
            }
            return BadRequest();
        }
    }
}