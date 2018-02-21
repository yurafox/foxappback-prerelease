using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.VARIANT_ITEMS")]
    [Serializable]
    public class Variant_Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Variant_Item()
        {
            PRODUCTS = new HashSet<Product>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long PROP1ID { get; set; }

        public long? PROP2ID { get; set; }

        public long? PROP3ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> PRODUCTS { get; set; }

        public virtual Prop PROP { get; set; }

        public virtual Prop PROP1 { get; set; }

        public virtual Prop PROP2 { get; set; }
    }
}
