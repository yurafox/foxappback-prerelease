using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PROPS")]
    [Serializable]
    public  class Prop
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prop()
        {
            CLIENT_PROP_VALUES = new HashSet<Client_Prop_Value>();
            PRODUCT_PROP_VALUES = new HashSet<Product_Prop_Value>();
            PRODUCT_TEMPLATE = new HashSet<Product_Template>();
            PROP_ENUMS_LISTS = new HashSet<Prop_Enums_List>();
            VARIANT_ITEMS = new HashSet<Variant_Item>();
            VARIANT_ITEMS1 = new HashSet<Variant_Item>();
            VARIANT_ITEMS2 = new HashSet<Variant_Item>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NAME { get; set; }

        public long? PROP_TYPE { get; set; }

        public bool? IS_MULTI_SELECT { get; set; }

        [StringLength(50)]
        public string URL { get; set; }

        public long? PREDESTINATION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Client_Prop_Value> CLIENT_PROP_VALUES { get; set; }

        public virtual Enum_Prop_Types ENUM_PROP_TYPES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Prop_Value> PRODUCT_PROP_VALUES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Template> PRODUCT_TEMPLATE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prop_Enums_List> PROP_ENUMS_LISTS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Variant_Item> VARIANT_ITEMS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Variant_Item> VARIANT_ITEMS1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Variant_Item> VARIANT_ITEMS2 { get; set; }
    }
}
