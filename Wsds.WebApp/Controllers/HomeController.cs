using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Wsds.WebApp.Infrastructure;

namespace Wsds.WebApp.Controllers
{
    public class HomeController : Controller
    {
        #region init cash on instance app
        //private ICacheService _product;
        //private IServiceCollection _collection;
        //private IConfigurationRoot _configuration;
        //private ICacheService<Product_Group> _productGroup;
        //private ICacheService<Product_Template> _template;
        #endregion

        #region ghost DI instance in .ctor
        public HomeController(
                              /*ICacheService product /*,
                              ICacheService<Product_Group> group,
                              ICacheService<Product_Template> template*/)
        {
            //var currency = AppDepResolver.GetEntityByName("currency");
            //_collection = collection;
            //collection.Where(sd=>sd.ImplementationType == typeof (ICacheService));
            //this._product = product;
            //this._productGroup = group;
            //this._template = template;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
    }
}