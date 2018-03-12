using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{
    public class DeliveryDateRequest
    {
        public long loEntity { get; set; }
        public long loIdClientAddress { get; set; }
        public ClientOrderProduct_DTO order { get; set; }
    }

    [Serializable]
    public class DeliveryRequestT22_S_Date
    {
        public long? g_id { get; set; }
        public long? iz_dozakupka { get; set; }
    }

    //[Serializable]
    //public class DeliveryRequestT22_Spec_Date
    //{
    //    public IEnumerable<DeliveryRequestT22_S_Date> s { get; set; }
    //}

    [Serializable]
    public class DeliveryRequestT22_Date
    {
        public long? sht_id { get; set; }
        public long? fcity_id { get; set; }
        public long? tcity_id { get; set; }
        public long? seller_id { get; set; }
        public long? type_deliv { get; set; }
        public IEnumerable<DeliveryRequestT22_S_Date> spec { get; set; }
    }

}
