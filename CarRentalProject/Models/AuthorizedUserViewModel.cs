using System.ComponentModel.DataAnnotations;

namespace CarRentalProject.Models
{
    public class AuthorizedUserViewModel
    {
        [Key]
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
    }
}
