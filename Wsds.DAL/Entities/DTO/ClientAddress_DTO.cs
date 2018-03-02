using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ClientAddress_DTO
    {
        public long? id { get; set; }
        public long? idClient { get; set; }
        public long? idCity { get; set; }
        public string zip { get; set; }
        public string street { get; set; }
        public decimal? lat { get; set; }
        public decimal? lng { get; set; }
        public bool? isPrimary { get; set; }
        public long? idCountry { get; set; }
        public string city { get; set; }
        public string bldApp { get; set; }
        public string recName { get; set; }
        public string phone { get; set; }
    }
}
