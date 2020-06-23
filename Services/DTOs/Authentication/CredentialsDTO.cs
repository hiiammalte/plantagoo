using System.ComponentModel.DataAnnotations;

namespace Plantagoo.DTOs.Authentication
{
    public class CredentialsDTO : PasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
