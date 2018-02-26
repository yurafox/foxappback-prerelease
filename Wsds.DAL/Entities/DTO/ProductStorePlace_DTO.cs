using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ProductStorePlace_DTO
    {
        public long? id { get; set; }
        public long? idQuotationProduct { get; set; }
        public long? idStorePlace { get; set; }
        public decimal? qty { get; set; }
    }
}
