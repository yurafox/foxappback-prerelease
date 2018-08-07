using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Newtonsoft.Json;
using Wsds.WebApp.Attributes;
using Wsds.WebApp.WebExtensions;
using Microsoft.AspNetCore.Authorization;
using Wsds.DAL.Entities.Communication;
using Wsds.WebApp.Filters;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LegalPolicyController : Controller
    {
        private readonly ILegalPolicyRepository _prodRepo;

        public LegalPolicyController(ILegalPolicyRepository prodRepo) {
            _prodRepo = prodRepo;
        }

        [HttpGet ("GetLegalPolicy/{id}")]
        public IActionResult GetLegalPolicy(long id) {
            return Ok(new {description = _prodRepo.GetLegalPolicy(id)});
        }
    }
}