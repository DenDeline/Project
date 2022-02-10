using AutoMapper;
using Sentaku.Infrastructure.Data;
using Sentaku.WebMVC.Models.Api.User;

namespace Sentaku.WebMVC.MappingProfiles
{
  public class AppUserProfile : Profile
  {
    public AppUserProfile()
    {
      CreateMap<AppUser, GetUserResponse>();
      CreateMap<AppUser, GetCurrentUserResponse>();
    }
  }
}
