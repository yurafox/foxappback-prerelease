using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Client_DTO
    {
        public long? id { get; set; }
        public long? userId { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string barcode { get; set; }
        public decimal? bonusBalance { get; set; }
        public decimal? actionBonusBalance { get; set; }
        public long? id_currency { get; set; }
        public long? id_lang { get; set; }
        public string appKey { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
