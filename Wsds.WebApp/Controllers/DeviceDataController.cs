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
    [Route("api/DeviceData")]
    public class DeviceDataController : Controller
    {
        private IDeviceDataRepository _ddRepo;
        private IClientRepository _cRepo;

        public DeviceDataController(IClientRepository cRepo, IDeviceDataRepository ddRepo)
        {
            _cRepo = cRepo;
            _ddRepo = ddRepo;
        }

        [Authorize]
        [HttpPost]
        [PullToken]
        public IActionResult SaveDeviceData([FromBody] DeviceData_DTO data)
        {
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    if (data != null)
                    {
                        DeviceData_DTO result = _ddRepo.SaveDeviceData(data, (long)client.id);
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