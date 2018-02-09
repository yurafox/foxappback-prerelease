using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.CREDIT_PRODUCTS")]
    public class Credit_Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_CREDIT_ENTITY { get; set; }

        public decimal FIRST_PMT { get; set; }

        public DateTime DATE_VALID_FROM { get; set; }

        public DateTime DATE_VALID_TO { get; set; }

        public decimal PCT_RATE { get; set; }

        public short MONTHS { get; set; }

        public virtual Credit_Entity CreditEntity { get; set; }
    }
}
