using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Wsds.DAL.Infrastructure.Facade;
using Wsds.WebApp.Infrastructure;

namespace Wsds.WebApp.Controllers
{
    public class HomeController : Controller
    {
        #region init cash on instance app
        private AccountUserFacade _accountFacade;
        #endregion

        #region ghost DI instance in .ctor
        public HomeController(AccountUserFacade accountFacade)
        {
            _accountFacade = accountFacade;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
    }
}