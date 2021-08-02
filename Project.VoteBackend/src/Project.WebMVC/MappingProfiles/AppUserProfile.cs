using AutoMapper;
using Project.ApplicationCore.Entities;
using Project.WebMVC.Models.Api.Users;

namespace Project.WebMVC.MappingProfiles
{
    public class AppUserProfile: Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, GetUserResponse>();
            CreateMap<AppUser, GetCurrentUserResponse>();
        }
    }
}
