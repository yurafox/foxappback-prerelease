using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.CURRENCY_RATES")]
    public  class Currency_Rate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long CUR1 { get; set; }

        public long CUR2 { get; set; }

        public DateTime ON_DATE { get; set; }

        public decimal RATE { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual Currency Currency1{ get; set; }
    }
}
