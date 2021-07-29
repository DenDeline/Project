using AutoMapper;
using Project.ApplicationCore.Dtos.AppUser;
using Project.ApplicationCore.Entities;

namespace Project.WebMVC.MappingProfiles
{
    public class AppUserProfile: Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserConfidentialReadDto>();
            CreateMap<AppUser, AppUserReadDto>();
        }
    }
}