using AutoMapper;
using Project.Infrastructure.Data;
using Project.WebMVC.Models.Api.User;

namespace Project.WebMVC.MappingProfiles
{
    public class AppUserProfile: Profile
    {
        public AppUserProfile()
        {
            CreateMap<ApplicationUser, GetUserResponse>();
            CreateMap<ApplicationUser, GetCurrentUserResponse>();
        }
    }
}
