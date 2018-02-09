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
    public class ProductCacheController : Controller
    {
        private readonly IDictionaryRepository _dictRepo;

        public ProductCacheController(IDictionaryRepository dictRepo)
        {
            _dictRepo = dictRepo;
        }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _dictRepo.ProductsCache;
        }

        [HttpGet("{id}")]
        public Product Get(int id)
        {
            return _dictRepo.ProductCache(id);
        }
    }
}