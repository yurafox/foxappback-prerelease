using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.LOCALES")]
    public class Locale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Locale()
        {
            LOCALIZATIONS = new HashSet<Localization>();
        }

        public byte ID { get; set; }

        [Required]
        [StringLength(20)]
        public string NAME { get; set; }

        public bool? IS_DEFAULT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Localization> LOCALIZATIONS { get; set; }
    }
}
