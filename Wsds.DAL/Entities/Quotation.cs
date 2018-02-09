using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.QUOTATIONS")]
    public class Quotation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quotation()
        {
            ACTION_OFFERS = new HashSet<Action_Offer>();
            OFFERS = new HashSet<Offer>();
            ORDER_SPEC_PRODUCTS = new HashSet<Order_Spec_Product>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_SUPPLIER { get; set; }

        public long ID_CUR { get; set; }

        public DateTime DATE_START { get; set; }

        public DateTime? DATE_END { get; set; }

        public long ID_PRODUCT { get; set; }

        public decimal PRICE { get; set; }

        public short? MAX_DELIVERY_DAYS { get; set; }

        public decimal? STOCK_QTY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Action_Offer> ACTION_OFFERS { get; set; }

        public virtual Currency CURRENCY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer> OFFERS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Spec_Product> ORDER_SPEC_PRODUCTS { get; set; }

        public virtual Product PRODUCT { get; set; }

        public virtual Supplier SUPPLIER { get; set; }
    }
}
