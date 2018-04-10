using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyRepository _repo;

        public CurrencyController(ICurrencyRepository repo, ICacheService<CurrencyRate_DTO> curRate) => _repo = repo;

        [HttpGet]
        public IActionResult Get() => Ok(_repo.Currencies);

        [HttpGet("{id}")]
        public IActionResult Get(long id) => Ok(_repo.Currency(id));

        //получение сортированного списка, вызываем через метод репозитория
        [HttpGet("GetCurAsc")]
        public IEnumerable<Currency> GetCurAsc()
        {
            return null; //_repo.CurAscending;
        }

        // получение таблицы курсов валют
        [HttpGet("rate")]
        public IActionResult GetRate()
        {
            var rateList = _repo.CurrencyRate;
            return (rateList != null) ? Ok(rateList) : (IActionResult)BadRequest();
        }

    }
}