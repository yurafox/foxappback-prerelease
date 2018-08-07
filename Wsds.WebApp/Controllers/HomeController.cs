using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Infrastructure.Facade;
using Serilog;
using Wsds.WebApp.TempTemplate;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    [Route("")]
    [Route("api")]
    [Route("api/v{version:apiVersion}")]
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
            //string dateTime = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");  
            //_serilog.Information($"===== The Index method of HomeController was invoked at {dateTime} =====");
            return View().GetRawContent(TemplateEnum.Home);
        }
    }
}