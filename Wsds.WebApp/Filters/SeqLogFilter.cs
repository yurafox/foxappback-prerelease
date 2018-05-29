using System;
using System.Reflection.Emit;
using System.Net;
using System.Net.Http;
using Serilog;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Wsds.WebApp.Filters
{
    public class SeqLogFilter : ExceptionFilterAttribute
    {
        ILogger serilogLogger { get; set; }
        
        public SeqLogFilter(ILogger logger)
        {
            serilogLogger = logger;
            serilogLogger.Information("serilog filter invoked");
        }

        public override void OnException(ExceptionContext context)
        {
            //base.OnException(context);
            var exception = context.Exception;
            context.ExceptionHandled = true;

            serilogLogger.Error("Serilog filter EXCEPTION");

            Debug.WriteLine("====================================");
            Debug.WriteLine("===== ERROR ========");
            Debug.WriteLine("====================================");

            var actionName = context.ActionDescriptor.DisplayName;
            context.Result = new ContentResult
            {
                Content = $"An error occurred in the {actionName} action",
                ContentType = "text/plain",
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
