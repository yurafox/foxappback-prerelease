using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;

namespace Wsds.WebApp.Controllers
{
    public class HomeController : Controller
    {
        #region init cash on instance app
        private ICacheService<Product> _product;
        private ICacheService<Product_Group> _productGroup;
        private ICacheService<Product_Template> _template;
        #endregion

        #region ghost DI instance in .ctor
        public HomeController(ICacheService<Product> product,
                              ICacheService<Product_Group> group,
                              ICacheService<Product_Template> template)
        {
            this._product = product;
            this._productGroup = group;
            this._template = template;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
    }
}