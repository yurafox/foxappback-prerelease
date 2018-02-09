using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCTS_IN_GROUPS")]
    public class Products_In_Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_GROUP { get; set; }

        public long ID_PRODUCT { get; set; }

        public virtual Product_Group ProductGroup { get; set; }

        public virtual Product PRODUCT { get; set; }
    }
}
