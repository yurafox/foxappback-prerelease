using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Wsds.DAL.Infrastructure.Facade;
using Wsds.WebApp.Infrastructure;
using Serilog;
using Wsds.WebApp.Filters;
using System;

namespace Wsds.WebApp.Controllers
{
    public class HomeController : Controller
    {
        #region init cash on instance app
        private AccountUserFacade _accountFacade;
        ILogger _serilog;
        #endregion

        #region ghost DI instance in .ctor
        public HomeController(AccountUserFacade accountFacade, ILogger logger)
        {
            _accountFacade = accountFacade;
            _serilog = logger;
        }
        #endregion

        public IActionResult Index()
        {
            string dateTime = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            _serilog.Information($"===== The Index method of HomeController was invoked at {dateTime} =====");

            return View();
        }
    }
}