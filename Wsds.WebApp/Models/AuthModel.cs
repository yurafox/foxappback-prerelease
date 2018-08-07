using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Models
{
    public class AuthModel
    {
        [Required]
        [RegularExpression("^380[0-9]{9}$")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"\d{3,6}")]
        public string Password { get; set; }

    }
}
