using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.CLIENT_PROP_VALUES")]
    public  class Client_Prop_Value
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_CLIENT { get; set; }

        public long ID_PROP { get; set; }

        [StringLength(255)]
        public string PROP_VALUE { get; set; }

        public virtual Enum_Prop_Types ENUM_PROP_TYPES { get; set; }

        public virtual Prop PROP { get; set; }

        public virtual Client CLIENT { get; set; }
    }
}
