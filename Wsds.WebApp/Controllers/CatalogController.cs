using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private ICacheService<Product_Group> _tst;

        public CatalogController(IDictionaryRepository dictionaryRepository, ICacheService<Product_Group> tst)
        {
            _dictionaryRepository = dictionaryRepository;
            _tst = tst;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var dataList= _dictionaryRepository
                .ProductsGroupsCache
                .Select(p => new { p.ID, p.NAME,
                                   parentId =p.PARENT_ID,
                                   priorityIndex =p.PRIORITY_INDEX,
                                   idProductCat = p.ID_PRODUCT_CAT,
                                   isShow= p.IS_SHOW,
                                   prefix = p.PREFIX,
                                   priorityShow =p.PRIORITY_SHOW,
                                   icon = (p.ICON != null) ? Convert.ToBase64String(p.ICON) : null  
                                 });

            return Ok(dataList);

        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var list = _dictionaryRepository
                              .ProductGroupsByFilterCache(p=>p.ID==id)
                              .Select(p=> new {p.ID,p.NAME,p.PARENT_ID});

            return (!list.Any()) ? (IActionResult) BadRequest($"can not find category tree by id={id}") 
                                  : Ok(list);

        }
    }
}