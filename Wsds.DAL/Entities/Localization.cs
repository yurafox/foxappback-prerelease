using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.LOCALIZATION")]
    public class Localization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ENTITY_ID { get; set; }

        public byte ID_LOCALE { get; set; }

        public long KEY_VALUE { get; set; }

        [Required]
        [StringLength(25)]
        public string COLUMN_NAME { get; set; }

        [Required]
        [StringLength(2000)]
        public string LOCALE_VALUE { get; set; }

        public virtual Entity Entity { get; set; }

        public virtual Locale LOCALE { get; set; }
    }
}
