using System;
using Microsoft.AspNetCore.Identity;

namespace Project.WebMVC.Identity
{
    public class AppUser: IdentityUser
    {
        public AppUser()
        {
            
        }
        
        public AppUser(string userName):
            base(userName)
        {
            
        }
        
        public string AvatarImageUrl { get; set; }
        public DateTime Birthday { get; set; }
    }
}