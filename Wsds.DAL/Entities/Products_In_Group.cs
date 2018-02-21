using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCTS_IN_GROUPS")]
    [Serializable]
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
