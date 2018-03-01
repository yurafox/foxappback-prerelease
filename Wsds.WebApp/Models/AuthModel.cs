using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Models
{
    public class AuthModel
    {
        [Required]
        [RegularExpression("^380[0 - 9]{9}$")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
