using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/MeasureUnit")]
    public class MeasureUnitController : Controller
    {
        private readonly IMeasureUnitRepository _repo;

        public MeasureUnitController(IMeasureUnitRepository repo) => _repo = repo;

        [HttpGet]
        public IActionResult Get() => Ok(_repo.MeasureUnits);

        [HttpGet("{id}")]
        public IActionResult Get(long id) => Ok(_repo.MeasureUnit(id));
    }
}