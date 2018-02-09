using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.ENUM_ORDER_PAYMENT_STATUS")]
    public  class Enum_Order_Payment_Status
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NAME { get; set; }
    }
}
