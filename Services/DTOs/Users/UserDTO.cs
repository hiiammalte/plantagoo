using System.ComponentModel.DataAnnotations;

namespace Plantagoo.DTOs.Users
{
    public class UserDTO
    {
        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
