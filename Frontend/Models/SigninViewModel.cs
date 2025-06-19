using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class SigninViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
