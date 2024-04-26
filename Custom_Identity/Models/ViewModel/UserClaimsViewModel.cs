using System.Security.Claims;

namespace Custom_Identity.Models.ViewModel
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel() 
        {
            Cliams = new List<UserClaim>();
        }

        public string UserId { get; set; }

        public List<UserClaim> Cliams { get; set; }
    }
}
