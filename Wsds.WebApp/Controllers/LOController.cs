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
    [Route("api/v{version:apiVersion}/[controller]")]
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
            return Ok(_loRepo.GetDeliveryCostByShipment(model.shpmt, model.loEntity, model.loIdClientAddress, model.delivTypeId));
        }

        [HttpPost("GetDeliveryDateByShipment")]
        public IActionResult GetDeliveryDateByShipment([FromBody] DeliveryDateByShipmentRequest model)
        {
            return Ok(_loRepo.GetDeliveryDateByShipment(model.shpmt, model.loEntity, model.loIdClientAddress, model.delivTypeId));
        }

        [HttpGet("LoDeliveryType/{id}")]
        public IActionResult GetLoDeliveryType(long id) => Ok(_loRepo.LoDeliveryType(id));

        [HttpGet("LoDeliveryTypesByLoEntity/{id}")]
        public IActionResult GetLoDeliveryTypesByLoEntity(long id) => Ok(_loRepo.GetLoDeliveryTypesByLoEntity(id));

        [HttpGet("LoEntityOffice/{id}")]
        public IActionResult GetLoEntityOffice(long id) => Ok(_loRepo.GetLoEntityOffice(id));

        [HttpPost("LoEntityOfficesByLoEntityAndCity")]
        public IActionResult GetLoEntityOfficesByLoEntityAndCity([FromBody] LoEntityOfficesByLoEntityAndCityRequest model) 
                => Ok(_loRepo.GetLoEntityOfficesByLoEntityAndCity(model.idLoEntity, model.idCity));

        [HttpPost("LoDeliveryTypesAttrByLoEntity")]
        public IActionResult GetLoDeliveryTypesAttrByLoEntity([FromBody] DeliveryRequest model)
        {
            return Ok(_loRepo.GetLoDeliveryTypesAttrByLoEntity(model.shpmt, model.loIdClientAddress));
        }

        [HttpGet("AllowTakeOnCredit/{status}")]
        public IActionResult AllowTakeOnCredit(long? status)
        {
            return Ok(_loRepo.AllowTakeOnCredit(status));
        }
    }
}