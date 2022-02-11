using AutoMapper;
using Sentaku.Infrastructure.Data;
using Sentaku.WebApi.Models.User;

namespace Sentaku.WebApi.MappingProfiles
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
