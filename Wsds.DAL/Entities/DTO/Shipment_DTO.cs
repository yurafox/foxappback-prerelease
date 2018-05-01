using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Shipment_DTO
    {
        public long id { get; set; }
        [FieldBinding(Field = "id_order")]
        public long idOrder { get; set; }
        [FieldBinding(Field = "id_supplier")]
        public long idSupplier { get; set; }
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
        [FieldBinding(Field = "id_store_place")]
        public long? idStorePlace { get; set; }
        [FieldBinding(Field = "id_lo_entity_office")]
        public long? idLoEntityOffice { get; set; }
        [FieldBinding(IsTransient = true)]
        public IEnumerable<Shipment_Items_DTO> shipmentItems { get; set; }
    }


}
