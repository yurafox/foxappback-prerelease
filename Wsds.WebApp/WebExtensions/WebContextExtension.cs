using Microsoft.AspNetCore.Http;
using Wsds.WebApp.Auth;

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
    }
}
