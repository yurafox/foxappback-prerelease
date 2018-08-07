using System;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class CreditCard_DTO
    {
        public long? id { get; set; }
        public long? id_client { get; set; }
        public string card_mask { get; set; }
        public string token { get; set; }
    }
}
