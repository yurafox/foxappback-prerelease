using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCT_TEMPLATE")]
    public  class Product_Template
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_Template()
        {
            PRODUCT_TEMPLATE_FUNC_AREA = new HashSet<Product_Template_Func_Area>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_PRODUCT_GROUP { get; set; }

        public long ID_PROP { get; set; }

        public bool? IS_MANDATORY { get; set; }

        public virtual Product_Group ProductGroup { get; set; }

        public virtual Prop PROP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Template_Func_Area> PRODUCT_TEMPLATE_FUNC_AREA { get; set; }
    }
}
