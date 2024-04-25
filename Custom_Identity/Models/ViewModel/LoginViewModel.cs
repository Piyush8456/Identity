using System.ComponentModel.DataAnnotations;

namespace Custom_Identity.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username Must Required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password Must Required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name="Remember Me")]
        public bool RememberMe { get; set;}
    }
}
