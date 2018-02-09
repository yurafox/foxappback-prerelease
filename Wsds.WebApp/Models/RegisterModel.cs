using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Models
{
    public class RegisterModel:AuthModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please type your Email")]
        [EmailAddress(ErrorMessage = "Email is't correct")]
        public string Email { get; set; }
    }
}
