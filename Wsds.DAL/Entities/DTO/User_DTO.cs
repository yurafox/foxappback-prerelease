using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    public class User_DTO
    {
        [Required]
        [StringLength(20)]
        public string name { get; set; }
        [Required]
        [RegularExpression("^380[0-9]{9}$")]
        public string phone { get; set; }

        public string login { get; set; }
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [StringLength(20)]
        public string fname { get; set; }
        [Required]
        [StringLength(20)]
        public string lname { get; set; }
        public string appKey { get; set; }
        [Required]
        public IDictionary<string,string> userSetting { get; set; }
        public long?[] favoriteStoresId { get; set; }

    }
}
