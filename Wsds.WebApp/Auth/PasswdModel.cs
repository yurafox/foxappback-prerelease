using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Auth
{
    public class PasswdModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Password { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
