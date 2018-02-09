using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCT_VIEW_HISTORY")]
    public class Product_View_History
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_CLIENT { get; set; }

        public long ID_PRODUCT { get; set; }

        public DateTime DATE_OF_VIEW { get; set; }

        public virtual Client CLIENT { get; set; }

        public virtual Product PRODUCT { get; set; }
    }
}
