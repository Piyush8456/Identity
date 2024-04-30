using System.Security.Claims;
using System.Collections.Generic;

namespace Custom_Identity.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
         
                new Claim("Create Role", "Create Role"),
                new Claim("Edit Role", "Edit Role"),
                new Claim("Delete Role", "Delete Role"),
                new Claim("ReadOnly Role", "ReadOnly Role")
        };
    }
}