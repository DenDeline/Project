using System;
using Microsoft.AspNetCore.Identity;

namespace Project.ApplicationCore.Entities
{
    public class AppUser: IdentityUser<int>
    {
        public AppUser() { }
        
        public AppUser(string userName): base(userName) { }
        
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Verified { get; set; }
        public string? ProfileImageUrl { get; set; } 
        public DateTime Birthday { get; set; }
        public int LanguageId { get; set; }
    }
}