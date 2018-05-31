using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;

namespace Wsds.WebApp.Filters
{
    public class CustomErrorFilter:ExceptionFilterAttribute
    {
        ILogger _serilog { get; set; }

        public CustomErrorFilter(ILogger logger)
        {
            _serilog = logger;
            _serilog.Warning("=====================================================================");
            _serilog.Warning("==== Serilog pushed this message. CustomErrorFilter was invoked. ====");
            _serilog.Warning("=====================================================================");
        }
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var actionName = context.ActionDescriptor.DisplayName;

            _serilog.Error($"Serilog pushed this message. An {exception.Message} Error occurred in the {actionName} action." +
                           $"That is the context of the Exception:" + Environment.NewLine + 
                           $"{ context.Exception}"
                           );
         
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            context.Result= new BadRequestObjectResult(exception.Message);
        }
    }
}

