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
    public class ClientOrderProduct_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field= "id_order")]
        public long? idOrder { get; set; }
        [FieldBinding(Field = "id_quotation")]
        public long? idQuotationProduct { get; set; }
        public decimal? price { get; set; }
        public decimal? qty { get; set; }
        [FieldBinding(Field = "id_store_place")]
        public long? idStorePlace { get; set; }
        [FieldBinding(Field = "id_lo_entity")]
        public long? idLoEntity { get; set; }
        [FieldBinding(Field = "lo_track_ticket")]
        public string loTrackTicket { get; set; }
        [FieldBinding(Field = "lo_delivery_cost")]
        public decimal? loDeliveryCost { get; set; }
        [FieldBinding(Field = "lo_delivery_completed")]
        public bool? loDeliveryCompleted { get; set; }
        [FieldBinding(Field = "lo_estimated_delivery_date")]
        public DateTime? loEstimatedDeliveryDate { get; set; }
        [FieldBinding(Field = "lo_delivery_completed_date")]
        public DateTime? loDeliveryCompletedDate { get; set; }
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
        [FieldBinding(Field = "earned_bonus_cnt")]
        public decimal? earnedBonusCnt { get; set; }
        public string complect { get; set; }
        [FieldBinding(Field = "id_action")]
        public long? idAction { get; set; }
        [FieldBinding(Field = "action_list")]
        public long? actionList { get; set; }
        [FieldBinding(Field = "action_title")]
        public string actionTitle { get; set; }
    }
}
