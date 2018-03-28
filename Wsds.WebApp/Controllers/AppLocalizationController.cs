using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/AppLocalization")]
    public class AppLocalizationController : Controller
    {
        private IAppLocalizationRepository _alRepo;

        public AppLocalizationController(IAppLocalizationRepository alRepo)
        {
            _alRepo = alRepo;
        }

        [HttpGet]
        public IActionResult GetAppLocalization()
        {
            return Ok(_alRepo.GetLocale());
        }

        //[HttpPost("PostLocale")]
        //public IActionResult SaveLocale([FromBody] Localization_DTO data)
        //{

        //    if (data != null)
        //    {
        //        Localization_DTO result = _alRepo.SaveLocalization(data);
        //        if (result != null)
        //        {
        //            return CreatedAtRoute("", result);
        //        }
        //    }
        //    return Ok(data.text);
        //}
    }
}