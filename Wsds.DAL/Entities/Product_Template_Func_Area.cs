using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCT_TEMPLATE_FUNC_AREA")]
    public  class Product_Template_Func_Area
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_SYSTEM { get; set; }

        public long ID_PRODUCT_TEMPLATE { get; set; }

        public short? LIST_INDEX { get; set; }

        public virtual Func_Area FuncArea { get; set; }

        public virtual Product_Template PRODUCT_TEMPLATE { get; set; }
    }
}
