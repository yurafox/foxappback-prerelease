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
        //static ILogger _logger;
        #endregion

        #region ghost DI instance in .ctor
        public HomeController(AccountUserFacade accountFacade)//, ILogger logger
        {
            _accountFacade = accountFacade;
            //_logger = logger;
        }
        #endregion

        public IActionResult Index()
        {
            //_logger.Information("Invoked Index method of HomeController");
        
            return View();
        }
    }
}