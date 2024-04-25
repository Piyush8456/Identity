using System.ComponentModel.DataAnnotations;

namespace Custom_Identity.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Password don't match")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword   { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        public string? Roles { get; set; }
    }
}
