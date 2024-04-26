using Microsoft.AspNetCore.Identity;

namespace Custom_Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        //public IList<string>? Roles { get; set; }
    }
}
