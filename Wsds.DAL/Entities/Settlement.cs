using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.SETTLEMENTS")]
    public class Settlement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long CUR { get; set; }

        public DateTime S_DATE { get; set; }

        public long ID_ORDER { get; set; }

        public decimal ID_USER { get; set; }

        public virtual Currency CURRENCY { get; set; }
    }
}
