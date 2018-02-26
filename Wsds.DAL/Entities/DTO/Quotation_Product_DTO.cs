using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Quotation_Product_DTO
    {
        public long id { get; set; }
        public long idQuotation { get; set; }
        public long idProduct { get; set; }
        public double? price { get; set; }
        public int? maxDeliveryDays { get; set; }
        public decimal? stockQuant { get; set; }
        public bool? stockLow { get; set; }
        public bool? freeShipping { get; set; }
        public decimal? actionPrice { get; set; }
    }
}
