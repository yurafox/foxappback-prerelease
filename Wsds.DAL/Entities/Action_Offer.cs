using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.ACTION_OFFERS")]
    public  class Action_Offer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_ACTION { get; set; }

        public long ID_QUOTATION { get; set; }

        public long ID_CUR { get; set; }

        public decimal ACTION_PRICE { get; set; }

        public virtual Entities.Action ACTION { get; set; }

        public virtual Currency CURRENCY { get; set; }

        //public virtual Quotation QUOTATION { get; set; }
    }
}
