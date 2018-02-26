using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class StorePlace_DTO
    {
        public long? id { get; set; }
        public long? idSupplier { get; set; }
        public string name { get; set; }
        public long? idCity { get; set; }
        public string zip { get; set; }
        public string address_line { get; set; }
        public decimal? lat { get; set; }
        public decimal? lng { get; set; }
        public long? type { get; set; }
    }
}
