using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.SERVICE_QUOTATIONS")]
    public class Service_Quotation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Service_Quotation()
        {
            ORDER_SPEC_SERVICES = new HashSet<Order_Spec_Service>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_SUPPLIER { get; set; }

        public long ID_CUR { get; set; }

        public DateTime DATE_START { get; set; }

        public DateTime? DATE_END { get; set; }

        public long ID_SERVICE { get; set; }

        public decimal PRICE { get; set; }

        public virtual Currency CURRENCY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Spec_Service> ORDER_SPEC_SERVICES { get; set; }

        public virtual Supplier SUPPLIER { get; set; }

        public virtual Service SERVICE { get; set; }
    }
}
