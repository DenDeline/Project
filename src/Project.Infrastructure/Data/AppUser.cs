using System;
using Microsoft.AspNetCore.Identity;
using Project.ApplicationCore.Entities;

namespace Project.Infrastructure.Data
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

        public string ProfileImageUrl { get; set; } =
            "https://i.pinimg.com/474x/57/70/f0/5770f01a32c3c53e90ecda61483ccb08.jpg";
        public DateTime Birthday { get; set; }
        
        public Language Language { get; set; }
    }
}