using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Project.WebMVC.Models.Api.User
{
  public class UpdateProfileImageRequest
  {
    [Required]
    public IFormFile FormFile { get; set; }
  }
}
