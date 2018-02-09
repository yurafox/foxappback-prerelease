using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.SERVICE_GROUPS")]
    public  class Service_Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [StringLength(255)]
        public string NAME { get; set; }

        public virtual Service SERVICE { get; set; }
    }
}
