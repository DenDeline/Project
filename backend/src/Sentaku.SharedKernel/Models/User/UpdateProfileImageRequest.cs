using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sentaku.SharedKernel.Models.User;

public class UpdateProfileImageRequest
{
  [Required]
  public IFormFile FormFile { get; set; }
}