using System;

namespace Project.ApplicationCore.Dtos.AppUser
{
    public class AppUserReadDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Verified { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Birthday { get; set; }
        public string RolesUrl { get; set; }
    }
}