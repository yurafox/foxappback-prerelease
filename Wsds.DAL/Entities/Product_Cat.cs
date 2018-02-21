using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCT_CATS")]
    [Serializable]
    public class Product_Cat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_Cat()
        {
            PRODUCT_GROUPS = new HashSet<Product_Group>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NAME { get; set; }

        public bool IS_MASTER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Group> PRODUCT_GROUPS { get; set; }
    }
}
