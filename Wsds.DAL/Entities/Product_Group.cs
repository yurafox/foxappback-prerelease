using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wsds.DAL.Common;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCT_GROUPS")]
    [Serializable]
    public class Product_Group :INamedEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_Group()
        {
            PRODUCT_TEMPLATE = new HashSet<Product_Template>();
            PRODUCTS_IN_GROUPS = new HashSet<Products_In_Group>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]
        [StringLength(255)]
        public string NAME { get; set; }

        public long? PARENT_ID { get; set; }

        public long ID_PRODUCT_CAT { get; set; }

        [StringLength(50)]
        public string PREFIX { get; set; }

        public byte[] ICON { get; set; }

        public bool IS_SHOW { get; set; }

        public long? PRIORITY_INDEX { get; set; }

        public long? PRIORITY_SHOW { get; set; }
  

        public virtual Product_Cat ProductCat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Template> PRODUCT_TEMPLATE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Products_In_Group> PRODUCTS_IN_GROUPS { get; set; }

        public string GetLocalizedName(int localId)
        {
            throw new NotImplementedException();
        }
    }
}
