using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Attributes;
using Wsds.WebApp.Models;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/LO")]
    public class LOController : Controller
    {
        private ILORepository _loRepo;
        public LOController(ILORepository loRepo) => _loRepo = loRepo;

        [HttpGet("LoEntity")]
        public IActionResult GetLoEntity() => Ok(_loRepo.LoEntities);

        [HttpGet("LoEntity/{id}")]
        public IActionResult GetLoEntityById(long id) => Ok(_loRepo.LoEntity(id));

        [HttpGet("LoSupplEntity")]
        [Link("idSupplier")]
        public IActionResult GetLoEntitiesForSuppl(long idSupplier) 
                                => Ok(_loRepo.GetLoEntitiesForSuppl(idSupplier));

        [HttpGet("SpecLOTrackingLog")]
        [Link("idOrderSpecProd")]
        public IActionResult GetTrackLogForOrderSpecProd(long idOrderSpecProd)
                                => Ok(_loRepo.GetTrackLogForOrderSpecProd(idOrderSpecProd));

        [HttpPost("GetDeliveryCost")]
        public IActionResult GetDeliveryCost([FromBody] DeliveryCostRequest model) {
            return Ok( _loRepo.GetDeliveryCost(model.order, model.loEntity, model.loIdClientAddress));
        }

        [HttpPost("GetDeliveryDate")]
        public IActionResult GetDeliveryDate([FromBody] DeliveryDateRequest model) {
            return Ok(_loRepo.GetDeliveryDate(model.order, model.loEntity, model.loIdClientAddress));
        }

    }
}