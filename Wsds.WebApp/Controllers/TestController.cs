using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    public class TestController : Controller
    {
        private readonly IDictionaryRepository _dbContext;
        private readonly IOrdersRepository _dbOrdersContext;
        /*
        private readonly ICacheService<Product> _csp;
        private readonly ICacheService<ProductGroup> _cspg;
        */

        public TestController(IDictionaryRepository context, IOrdersRepository ordContext /*, ICacheService<Product> csp, ICacheService<ProductGroup> cspg*/)

        {
            _dbContext = context;
            _dbOrdersContext = ordContext;
            /*
            _csp = csp;
            _cspg = cspg;
            */
        }

        public ActionResult Index()
        {
            //return View(_dbContext.Currencies);
            return View(_dbOrdersContext.ClientOrders);
        }

        [HttpGet]
        public Product Product(int id)
        {
            return _dbContext.Product(id);
        }
        public ActionResult Products([FromServices] ICacheService<Product> csp)
        {
            return View(csp.Items);
        }

        public ActionResult ProductGroups([FromServices] ICacheService<Product_Group> cspg)
        {
            return View(cspg.Items);
        }


    }
}