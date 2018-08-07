using System;
using System.Globalization;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class LoDeliveryTypeAttr_DTO
    {
        public long loEntityId { get; set; }
        public long deliveryTypeId { get; set; }
        public DateTime deliveryDate { get; set; }

        public LoDeliveryTypeAttr_DTO(long _loEntityId, long _deliveryTypeId, string _deliveryDate)
        {
            loEntityId = _loEntityId;
            deliveryTypeId = _deliveryTypeId;
            deliveryDate = DateTime.ParseExact(_deliveryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}