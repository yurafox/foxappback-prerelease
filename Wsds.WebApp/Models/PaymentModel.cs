using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Models
{
    public class PaymentModel
    {
        [Required(ErrorMessage = "Please enter order id value")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid order id")]
        public int Id { get; set; } // orderId

        [Required(ErrorMessage = "Please enter total value")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid cart total")]
        public int Total { get; set; } // cartTotal
    }
}
