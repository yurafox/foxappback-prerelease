﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Product_DTO
    {
        public long id { get; set; }
        public string name { get; set; }
        public decimal? price { get; set; }
        public decimal? oldPrice { get; set; }
        public decimal? bonuses { get; set; }
        public long? manufacturerId { get; set; }
        public ICollection<Product_Prop_Value_DTO> props { get; set; }
        public string imageUrl { get; set; }
        public decimal? rating { get; set; }
        public int? recall { get; set; }
        public int? supplOffers { get; set; }
        public string description { get; set; }
        public ICollection<string> slideImageUrls { get; set; }
        public string barcode { get; set; }
        public long? valueQP { get; set; }
        public long? status { get; set; }
        public long? site_status { get; set; }
    }
}
