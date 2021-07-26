using AutoMapper;
using Project.ApplicationCore.Dtos.AppUser;
using Project.ApplicationCore.Entities;

namespace Project.WebMVC.MappingProfiles
{
    public class AppUserProfile: Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserConfidentialReadDto>()
                .ForMember(_ => _.RolesUrl, 
                    expression => expression.MapFrom(user => ApiEndpoints.Users.GetUserRoles(user.UserName)));
            CreateMap<AppUser, AppUserReadDto>()
                .ForMember(_ => _.RolesUrl, 
                    expression => expression.MapFrom(user => ApiEndpoints.Users.GetUserRoles(user.UserName)));;
        }
    }
}