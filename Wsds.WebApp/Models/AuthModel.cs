using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Models
{
    public class AuthModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
