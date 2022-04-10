using System.ComponentModel.DataAnnotations;

namespace Sentaku.AuthServer.ViewModels;

public class LoginViewModel
{
  public string? ReturnTo { get; set; }
  
  [Required]
  public string Login { get; set; }
  
  [Required]
  [DataType(DataType.Password)]
  public string Password { get; set; }
}
