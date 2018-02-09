using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.ORDER_SPEC_PRODUCTS")]
    public class Order_Spec_Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_ORDER { get; set; }

        public long ID_QUOTATION { get; set; }

        public decimal PRICE { get; set; }

        public decimal QTY { get; set; }

        public virtual Client_Order CLIENT_ORDERS { get; set; }

        public virtual Quotation QUOTATION { get; set; }
    }
}
