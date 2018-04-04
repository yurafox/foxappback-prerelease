using System;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class CurrencyRate_DTO
    {
        public int defaultId { get; set; }
        public int targetId { get; set; }
        public decimal rate { get; set; }
    }
}
