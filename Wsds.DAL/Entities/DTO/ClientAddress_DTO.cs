using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ClientAddress_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "ID_CLIENT")]
        public long? idClient { get; set; }
        [FieldBinding(Field = "ID_CITY")]
        public long? idCity { get; set; }
        public string zip { get; set; }
        public string street { get; set; }
        public decimal? lat { get; set; }
        public decimal? lng { get; set; }
        [FieldBinding(Field = "IS_PRIMARY")]
        public bool? isPrimary { get; set; }
        [FieldBinding(Field = "ID_COUNTRY")]
        public long? idCountry { get; set; }
        public string city { get; set; }
        [FieldBinding(Field = "BLD_APP")]
        public string bldApp { get; set; }
        public string recName { get; set; }
        public string phone { get; set; }
    }
}
