using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Infrastructure
{
    public static class UrlConstants
    {
        public static string GetBonusInfoUrl = "http://hybrisservices.mc.gcf/api/Loyality/GetBalance";
        public static string GetClientBonusesExpireInfoUrl = "http://hybrisservices.mc.gcf/api/Loyality/GetBonuseList";
    }
}
