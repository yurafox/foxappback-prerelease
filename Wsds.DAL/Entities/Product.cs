using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCTS")]
    public class Product
    {
        public Product()
        {
            PRODUCT_FILES = new HashSet<Product_File>();
            PRODUCT_PROP_VALUES = new HashSet<Product_Prop_Value>();
            PRODUCT_VIEW_HISTORY = new HashSet<Product_View_History>();
            PRODUCTS_IN_GROUPS = new HashSet<Products_In_Group>();
            QUOTATIONS = new HashSet<Quotation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]
        [StringLength(255)]
        public string NAME { get; set; }

        public long ID_MANUFACTURER { get; set; }

        public long? ID_VARIANT_ITEM { get; set; }

        [StringLength(150)]
        public string URL { get; set; }

        public virtual Manufacturer MANUFACTURER { get; set; }

 
        public virtual ICollection<Product_File> PRODUCT_FILES { get; set; }


        public virtual ICollection<Product_Prop_Value> PRODUCT_PROP_VALUES { get; set; }

   
        public virtual ICollection<Product_View_History> PRODUCT_VIEW_HISTORY { get; set; }

        public virtual Variant_Item VariantItem { get; set; }


        public virtual ICollection<Products_In_Group> PRODUCTS_IN_GROUPS { get; set; }


        public virtual ICollection<Quotation> QUOTATIONS { get; set; }
    }
}
