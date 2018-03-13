using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            var tokenModel= new TokenModel()
            {
                Phone = phone,
                Card = (card != null) ? Convert.ToInt64(card) : 0,
                ClientId = (clientId != null) ? Convert.ToInt64(clientId) : 0,
            };

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
