using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{
    public class CalculateCartRequest
    {
        public string promoCode { get; set; }
        public decimal? maxBonusCnt { get; set; }
        public bool? usePromoBonus { get; set; }
        public long? creditProductId { get; set; }
        //public IEnumerable<ClientOrderProduct_DTO> cartContent { get; set; }
    }

    [Serializable]
    public class CalcCartRequestT22_Item
    {
        public long? pk_id { get; set; }
        public long? g_id { get; set; }
        public decimal? qty { get; set; }
        public decimal? price { get; set; }
        public long? is_set { get; set; }
        public long? act { get; set; }
    }

    //[Serializable]
    //public class CalcCartRequestT22_S
    //{
    //    public IEnumerable<CalcCartRequestT22_Item> s { get; set; }
    //}

    [Serializable]
    public class CalcCartRequestT22
    {
        public string cnum { get; set; }
        public long? lptype { get; set; }
        public string promocode { get; set; }
        public decimal? bonpayamm { get; set; }
        public long? dogrole { get; set; }
        public long? is_cred_prod { get; set; }
        public long? bait_bon { get; set; }

        public IEnumerable<CalcCartRequestT22_Item> spec { get; set; }
    }
}
