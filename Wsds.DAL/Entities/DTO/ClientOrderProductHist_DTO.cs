using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ClientOrderProductHist_DTO: OrderSpecProductBase
    {
        [FieldBinding(Field = "id_lo_entity", IsTransient = true)]
        public long? idLoEntity { get; set; }
        [FieldBinding(Field = "lo_track_ticket", IsTransient = true)]
        public string loTrackTicket { get; set; }
        [FieldBinding(Field = "lo_delivery_cost", IsTransient = true)]
        public decimal? loDeliveryCost { get; set; }
        [FieldBinding(Field = "lo_delivery_completed", IsTransient = true)]
        public bool? loDeliveryCompleted { get; set; }
        [FieldBinding(Field = "lo_estimated_delivery_date", IsTransient = true)]
        public DateTime? loEstimatedDeliveryDate { get; set; }
        [FieldBinding(Field = "lo_delivery_completed_date", IsTransient = true)]
        public DateTime? loDeliveryCompletedDate { get; set; }
    }
}
