using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.CLIENT_ORDERS")]
    public class Client_Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client_Order()
        {
            ORDER_SPEC_PRODUCTS = new HashSet<Order_Spec_Product>();
            ORDER_SPEC_SERVICES = new HashSet<Order_Spec_Service>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public DateTime ORDER_DATE { get; set; }

        public long CUR { get; set; }

        public long ID_CLIENT { get; set; }

        public decimal TOTAL { get; set; }

        public long ID_PAYMENT_METHOD { get; set; }

        public long ID_PAYMENT_STATUS { get; set; }

        public long? STATUS { get; set; }

        public virtual Currency CURRENCY { get; set; }

        public virtual Client CLIENT { get; set; }

        public virtual Enum_Payment_Method ENUM_PAYMENT_METHODS { get; set; }

        public virtual Enum_Order_Status ENUM_ORDER_STATUS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Spec_Product> ORDER_SPEC_PRODUCTS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Spec_Service> ORDER_SPEC_SERVICES { get; set; }
    }
}
