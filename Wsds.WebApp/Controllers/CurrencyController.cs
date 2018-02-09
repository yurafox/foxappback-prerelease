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
        private readonly IDictionaryRepository _repo;

        public CurrencyController(IDictionaryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<Currency> Get()
        {
            return _repo.Currencies;
        }

        [HttpGet("{id}")]
        public Currency Get(int id)
        {
            return _repo.Currency(id);
        }

        //получение сортированного списка, вызываем через метод репозитория
        [HttpGet("GetCurAsc")]
        public IEnumerable<Currency> GetCurAsc()
        {
            return _repo.CurAscending;
        }

    }
}