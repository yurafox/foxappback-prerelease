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
    [Route("api/Geo")]
    public class GeoController : Controller
    {
        private IGeoRepository _repo;

        public GeoController(IGeoRepository repo) => _repo = repo;

        [HttpGet("City/{id}")]
        public IActionResult GetCity(long id) => Ok(_repo.City(id));

        [HttpGet("City")]
        public IActionResult GetCities() => Ok(_repo.Cities);

        [HttpGet("Country/{id}")]
        public IActionResult GetCountry(long id) => Ok(_repo.Country(id));

        [HttpGet("Country")]
        public IActionResult GetCountries() => Ok(_repo.Countries);

        [HttpGet("citiesWithStores")]
        public IActionResult GetCitiesWithStores() => Ok(_repo.CitiesWithStores());

        [HttpGet("City")]
        [Link("srch")]
        public IActionResult SearchCities([FromQuery] string srch)
        {
            return Ok(_repo.SearchCities(srch));
        }

        [HttpGet("Region/{id}")]
        public IActionResult GetRegion(long id) => Ok(_repo.Region(id));

        [HttpGet("Region")]
        public IActionResult GetRegions() => Ok(_repo.Regions);
    }
}