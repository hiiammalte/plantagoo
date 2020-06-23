using System.ComponentModel.DataAnnotations;

namespace Plantagoo.DTOs.Authentication
{
    public class PasswordDTO
    {
        [Required]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Please specify a password between 8 and 32 characters.")]
        public string Password { get; set; }
    }
}
