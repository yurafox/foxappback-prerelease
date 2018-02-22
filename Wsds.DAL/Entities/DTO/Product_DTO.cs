using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Product_DTO : IDTO
    {
        public long id { get; set; }
        public string name { get; set; }
        public decimal? price { get; set; }
        public decimal? oldPrice { get; set; }
        public decimal? bonuses { get; set; }
        public long? manufacturerId { get; set; }
        public ICollection<Product_Prop_Value_DTO> Props { get; set; }
        public string imageUrl { get; set; }
        public int? rating { get; set; }
        public int? recall { get; set; }
        public int? supplOffers { get; set; }
        public ICollection<string> slideImageUrls { get; set; }
        public string barcode { get; set; }
    }
}
