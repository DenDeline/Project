using System.Collections.Generic;
using System.Security.Claims;

namespace Project.WebMVC.ViewModels
{
    public class StatusViewModel
    {
        public string Username { get; set; }
        public IReadOnlyList<Claim> UserClaims { get; set; }
        public bool IsUserAuthenticated { get; set; }
        public string AuthenticationMethod { get; set; }
    }
}