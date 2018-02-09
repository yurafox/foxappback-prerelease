using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.FUNC_AREAS")]
    public class Func_Area
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Func_Area()
        {
            PRODUCT_TEMPLATE_FUNC_AREA = new HashSet<Product_Template_Func_Area>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string NAME { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Template_Func_Area> PRODUCT_TEMPLATE_FUNC_AREA { get; set; }
    }
}
