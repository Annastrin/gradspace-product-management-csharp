using System.ComponentModel.DataAnnotations;

namespace ProductManagementBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
