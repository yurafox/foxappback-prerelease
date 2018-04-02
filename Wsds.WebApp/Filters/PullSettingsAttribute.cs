using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Wsds.WebApp.Auth;

namespace Wsds.WebApp.Filters
{
    public class PullSettingsAttribute:Attribute,IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            var currency = headers["X-Currency"].FirstOrDefault();

            // get lang from current instance
            var config = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var lang = config?["AppOptions:lang"];

            // create token model
            var settingsModel = new SettingsModel()
            {
                CurrencyId = (currency != null) ? Convert.ToInt64(currency) : 4, // default UAH
                LangId = (lang != null) ? Convert.ToInt64(lang) : 1 // default RUS
            };

            context.HttpContext.Items["settings"] = settingsModel;       

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Items["settings"] = null;
        }
    }
}
