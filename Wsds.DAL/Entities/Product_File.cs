using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCT_FILES")]
    public class Product_File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_PRODUCT { get; set; }

        public byte[] CONTENT { get; set; }

        public bool? IS_IMAGE { get; set; }

        public short? LIST_INDEX { get; set; }

        public virtual Product PRODUCT { get; set; }
    }
}
