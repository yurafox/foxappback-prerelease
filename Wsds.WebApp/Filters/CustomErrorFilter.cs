using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wsds.WebApp.Filters
{
    public class CustomErrorFilter:ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            context.Result= new BadRequestObjectResult(exception.Message);
        }
    }
}

