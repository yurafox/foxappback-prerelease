using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyRepository _repo;

        public CurrencyController(ICurrencyRepository repo) => _repo = repo;

        [HttpGet]
        public IActionResult Get() => Ok(_repo.Currencies);

        [HttpGet("{id}")]
        public IActionResult Get(int id) => Ok(_repo.Currency(id));

        //получение сортированного списка, вызываем через метод репозитория
        [HttpGet("GetCurAsc")]
        public IEnumerable<Currency> GetCurAsc()
        {
            return null; //_repo.CurAscending;
        }

    }
}