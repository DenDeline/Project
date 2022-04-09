using AutoMapper;
using Sentaku.Infrastructure.Data;
using Sentaku.WebApi.Models.User;

namespace Sentaku.WebApi.MappingProfiles
{
  public class AppUserProfile : Profile
  {
    public AppUserProfile()
    {
      // CreateMap<AppUser, GetUserResponse>()
      //   .ForMember(_ => _.Id, expression => expression.MapFrom(user => user.Id))
      //   .ForMember(_ => _.Name, expression => expression.MapFrom(user => user.Name))
      //   .ForMember(_ => _.Surname, expression => expression.MapFrom(user => user.Surname))
      //   .ForMember(_ => _.Username, expression => expression.MapFrom(user => user.UserName))
      //   .ForMember(_ => _.Verified, expression => expression.MapFrom(user => user.Verified))
      //   .ForMember(_ => _.CreatedAt, expression => expression.MapFrom(user => user.CreatedAt));

      CreateProjection<AppUser, GetUserResponse>();
      
      CreateMap<AppUser, GetCurrentUserResponse>();
      CreateMap<AppUser, GetUserResponse>();
    }
  }
}
