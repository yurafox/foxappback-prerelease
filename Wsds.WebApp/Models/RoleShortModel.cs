using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Models
{
    public class RoleShortModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
