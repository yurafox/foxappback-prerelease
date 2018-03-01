using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    public class User_DTO
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string appKey { get; set; }
        public IDictionary<string,string> userSetting { get; set; }
        public long?[] favoriteStoresId { get; set; }

    }
}
