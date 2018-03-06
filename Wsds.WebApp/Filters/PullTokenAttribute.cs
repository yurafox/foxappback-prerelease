using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Wsds.WebApp.Auth;

namespace Wsds.WebApp.Filters
{
    public class PullTokenAttribute:Attribute,IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var phone = context.HttpContext.User.FindFirst("userName")?.Value;
            var client = context.HttpContext.User.FindFirst("clientId")?.Value;
            context.HttpContext.Items["token"] = new TokenModel()
            {
                Phone = phone,
                Client = (client!= null) ? Convert.ToInt64(client) : 0
            };
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Items["token"] = null;
        }
    }
}
