using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Attributes;
using Microsoft.AspNetCore.Authorization;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/StorePlace")]
    public class StorePlaceController : Controller
    {
        private IStorePlaceRepository _spRepo;
        private IClientRepository _cRepo;

        public StorePlaceController(IClientRepository cRepo, IStorePlaceRepository spRepo)
        {
            _cRepo = cRepo;
            _spRepo = spRepo;
        }

        [HttpGet("Stores")]
        public IActionResult GetStores()
        {
            return Ok(_spRepo.Stores);
        }

        [HttpGet("Store/{id}")]
        public IActionResult GetStore(long id)
        {
            return Ok(_spRepo.GetStore(id));
        }

        [HttpGet("StorePlace/{id}")]
        public IActionResult GetStorePlaceById(long id) {
            return Ok(_spRepo.StorePlace(id));
        }

        [HttpGet("ProductStorePlaces")]
        [Link("idQuotationProduct")]
        public IActionResult GetProductStorePlacesByQuotId([FromQuery] long idQuotationProduct)
        {
            return Ok(_spRepo.GetProductSPByQuotId(idQuotationProduct));
        }

        [HttpGet("StorePlace")]
        public IActionResult GetStorePlaces()
        {
            return Ok(_spRepo.StorePlaces);
        }

        [Authorize]
        [HttpGet("GetFavoriteStores")]
        [PullToken]
        public IActionResult GetFavoriteStores()
        {
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    return Ok(_spRepo.GetFavoriteStores((long)client.id));
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("AddFavoriteStore/{id}")]
        [PullToken]
        public IActionResult AddFavoriteStore(long id)
        {
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null && 0 < id)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    long result = _spRepo.AddFavoriteStore(id, (long)client.id);
                    if (0 < result)
                    {
                        return StatusCode(201, result);
                    }
                    else return StatusCode(200, 0);
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("DeleteFavoriteStore/{id}")]
        [PullToken]
        public IActionResult DeleteFavoriteStore(long id)
        {
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null && 0 < id)
            {
                var client = _cRepo.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    long result = _spRepo.DeleteFavoriteStore(id, (long)client.id);
                    if (0 < result)
                    {
                        return Ok(result);
                    }
                    else return StatusCode(200, 0);
                }
                return BadRequest();
            }
            return BadRequest();
        }
    }
}