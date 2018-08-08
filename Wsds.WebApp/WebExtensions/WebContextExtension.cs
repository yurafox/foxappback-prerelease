using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wsds.WebApp.Auth;
using Wsds.WebApp.TempTemplate;

namespace Wsds.WebApp.WebExtensions
{
    public static class WebContextExtension
    {
        public static TokenModel GetTokenModel(this HttpContext context)
        {
            return context.Items["token"] as TokenModel;
        }

        public static SettingsModel GetSettingsModel(this HttpContext context)
        {
            return context.Items["settings"] as SettingsModel;
        }

        public static ContentResult GetRawContent(this ViewResult result, TemplateEnum template, object model=null)
        {
            var templFact = new TemplateFactory();
            var content = templFact.GetTemplate(template,model);
            return new ContentResult() { Content = content, ContentType = "text/html"};
        }
    }
}
