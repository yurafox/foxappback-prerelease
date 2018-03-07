using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wsds.WebApp.Auth
{
    public class TokenModel
    {
        public string Phone { get; set; }
        public long Card { get; set; }
        public long ClientId { get; set; }

        public bool ValidateDataFromToken()
        {
            return (!String.IsNullOrEmpty(Phone) && Card != 0 && ClientId !=0);
        }
    }
}
