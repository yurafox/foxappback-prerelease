using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.OFFERS")]
    public  class Offer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_CLIENT { get; set; }

        public long ID_QUOTATION { get; set; }

        public decimal PRICE { get; set; }

        public DateTime? DATE_VALID_FROM { get; set; }

        public DateTime? DATE_VALID_TO { get; set; }

        public virtual Client CLIENT { get; set; }

        //public virtual Quotation QUOTATION { get; set; }
    }
}
