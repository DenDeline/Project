using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sentaku.WebApi.Models.User;

public class UpdateProfileImageRequest
{
  [Required]
  public IFormFile FormFile { get; set; }
}