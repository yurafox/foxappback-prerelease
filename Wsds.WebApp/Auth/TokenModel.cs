using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wsds.WebApp.Auth
{
    public class TokenModel
    {
        public string Phone { get; set; }
        public long Client { get; set; }

        public bool ValidateDataFromToken()
        {
            return (!String.IsNullOrEmpty(Phone) && Client != 0);
        }
    }
}
