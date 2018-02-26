using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Quotation_DTO
    {
        public long? id { get; set; }
        public long? idSupplier { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public long? currencyId { get; set; }
    }
}
