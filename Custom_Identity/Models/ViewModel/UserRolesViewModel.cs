using Custom_Identity.Models;
using System.Collections.Generic;

namespace Custom_Identity.Models
{
    public class UserRolesViewModel 
   {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
