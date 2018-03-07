using Microsoft.AspNetCore.Http;
using Wsds.WebApp.Auth;

namespace Wsds.WebApp.WebExtensions
{
    public static class WebContextExtension
    {
        public static TokenModel GeTokenModel(this HttpContext context)
        {
            return context.Items["token"] as TokenModel;
        }
    }
}
