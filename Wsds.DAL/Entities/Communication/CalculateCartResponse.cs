using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{
    public class CalculateCartResponse
    {
        public long? clOrderSpecProdId { get; set; }
        public decimal? promoCodeDisc { get; set; }
        public decimal? bonusDisc { get; set; }
        public decimal? promoBonusDisc { get; set; }
        public decimal? earnedBonus { get; set; }
    }

    [Serializable]
    public class CalcCartResponseT22_Item
    {
        public string pk_id { get; set; }
        public long? g_id { get; set; }
        public decimal? qty { get; set; }
        public decimal? price { get; set; }
        public decimal? bonus { get; set; }
        public long? is_set { get; set; }
        public long? action_id { get; set; }
        public long? type_action { get; set; }
        public long? action_list { get; set; }
        public decimal? bonus_pay { get; set; }
        public string promocode { get; set; }
        public decimal? bonus_sp_pay { get; set; }
    }

    //[Serializable]
    //public class CalcCartResponseT22_S
    //{
    //    public IEnumerable<CalcCartResponseT22_Item> s { get; set; }
    //}

    [Serializable]
    public class CalcCartResponseT22
    {
        public string msg { get; set; }
        public decimal? actbonamm { get; set; }
        public decimal? gperc { get; set; }
        public IEnumerable<CalcCartResponseT22_Item> outspec { get; set; }
    }
}
