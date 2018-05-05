using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Wsds.WebApp.Auth;

namespace Wsds.WebApp.Filters
{
    public class PullTokenAttribute:Attribute,IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var phone = context.HttpContext.User.FindFirst("phone")?.Value;
            var card = context.HttpContext.User.FindFirst("card")?.Value;
            var clientId = context.HttpContext.User.FindFirst("clientId")?.Value;

            // common settings
            var headers = context.HttpContext.Request.Headers;
            var currency = headers["X-Currency"].FirstOrDefault();
            var idApp = headers["X-App"].FirstOrDefault();

            // get lang from current instance
            var config = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var lang = config?["AppOptions:lang"];

            // create token model
            var tokenModel= new TokenModel()
            {
                Phone = phone,
                Card = (card != null) ? Convert.ToInt64(card) : 0,
                ClientId = (clientId != null) ? Convert.ToInt64(clientId) : 0,
                CurrencyId = (currency != null) ? Convert.ToInt64(currency) : 4, // default UAH
                LangId = (lang != null) ? Convert.ToInt64(lang) : 1, // default RUS
                IdApp = (idApp != null) ? Convert.ToInt64(idApp) : 0
            };

            // add token model to request like temp object
            if (tokenModel.ValidateDataFromToken())
                context.HttpContext.Items["token"] = tokenModel;
            else 
                context.Result = new UnauthorizedResult();

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Items["token"] = null;
        }
    }
}
