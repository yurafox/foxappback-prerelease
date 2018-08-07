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
    public class ClientOrderProduct_DTO: OrderSpecProductBase
    {
        [FieldBinding(Field = "error_message")]
        public string errorMessage { get; set; }
        [FieldBinding(Field = "warning_message")]
        public string warningMessage { get; set; }
        [FieldBinding(Field = "warning_read")]
        public bool? warningRead { get; set; }
        [FieldBinding(Field = "pay_promocode")]
        public string payPromoCode { get; set; }
        [FieldBinding(Field = "pay_promocode_discount")]
        public decimal? payPromoCodeDiscount { get; set; }
        [FieldBinding(Field = "pay_bonus_cnt")]
        public decimal? payBonusCnt { get; set; }
        [FieldBinding(Field = "pay_promobonus_cnt")]
        public decimal? payPromoBonusCnt { get; set; }
        public string complect { get; set; }
        [FieldBinding(Field = "id_action")]
        public long? idAction { get; set; }
        [FieldBinding(Field = "action_list")]
        public long? actionList { get; set; }
        [FieldBinding(Field = "action_title")]
        public string actionTitle { get; set; }
        [FieldBinding(IsTransient = true)]
        public Quotation_Product_DTO quotationProduct;
    }
}
