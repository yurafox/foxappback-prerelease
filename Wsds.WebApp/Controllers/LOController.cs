using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Attributes;
using Wsds.WebApp.Filters;
using Wsds.WebApp.Models;
using Wsds.WebApp.WebExtensions;

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

        [HttpPost("GetDeliveryCostByShipment")]
        public IActionResult GetDeliveryCostByShipment([FromBody] DeliveryCostByShipmentRequest model)
        {
            return Ok(_loRepo.GetDeliveryCostByShipment(model.shpmt, model.loEntity, model.loIdClientAddress));
        }

        [HttpPost("GetDeliveryDateByShipment")]
        public IActionResult GetDeliveryDateByShipment([FromBody] DeliveryDateByShipmentRequest model)
        {
            return Ok(_loRepo.GetDeliveryDateByShipment(model.shpmt, model.loEntity, model.loIdClientAddress));
        }



    }
}