using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.DELIVERY_OPTIONS")]
    public class Delivery_Option
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_SUPPLIER { get; set; }

        [StringLength(20)]
        public string COLUMN1 { get; set; }

        public virtual Supplier SUPPLIER { get; set; }
    }
}
