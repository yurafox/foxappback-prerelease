using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Entities.Communication
{
    public class DeliveryCostByShipmentRequest
    {
        public long loEntity { get; set; }
        public long loIdClientAddress { get; set; }
        public Shipment_DTO shpmt { get; set; }
        public long delivTypeId { get; set; }
    }


    public class DeliveryCostRequest
    {
        public long loEntity { get; set; }
        public long loIdClientAddress { get; set; }
        public ClientOrderProduct_DTO order { get; set; }
    }

    [Serializable]
    public class DeliveryRequestT22_S_Cost
    {
        public long? g_id { get; set; } 
        public decimal? qty { get; set; }
        public decimal? price { get; set; }
     }

    [Serializable]
    public class DeliveryRequestT22_Cost
    {
        public long? sht_id { get; set; }
        public long? tcity_id { get; set; }
        public long? seller_id { get; set; }
        public long? numfloor { get; set; }
        public long? type_deliv { get; set; }
        public IEnumerable<DeliveryRequestT22_S_Cost> spec { get; set; } 
    }  
}
