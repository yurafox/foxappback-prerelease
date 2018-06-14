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
        public static string GetBonusesExpireInfoUrl = "http://hybrisservices.mc.gcf/api/Loyality/GetBonuseList";
        public static string CallMeServiceUrl = "http://10.253.79.6:8002/callHandler.ashx?number={0}+2000000&queueId=1";
    }
}
