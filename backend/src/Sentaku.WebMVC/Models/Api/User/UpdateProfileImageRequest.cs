using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sentaku.WebMVC.Models.Api.User
{
  public class UpdateProfileImageRequest
  {
    [Required]
    public IFormFile FormFile { get; set; }
  }
}
