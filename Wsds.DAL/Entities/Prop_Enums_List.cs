using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PROP_ENUMS_LISTS")]
    [Serializable]
    public  class Prop_Enums_List
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prop_Enums_List()
        {
            PRODUCT_PROP_VALUES = new HashSet<Product_Prop_Value>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_PROP { get; set; }

        [Required]
        [StringLength(50)]
        public string NAME { get; set; }

        public short? LIST_INDEX { get; set; }

        public byte? BIT_INDEX { get; set; }

        [StringLength(50)]
        public string URL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Prop_Value> PRODUCT_PROP_VALUES { get; set; }

        public virtual Prop PROP { get; set; }
    }
}
