using System.ComponentModel.DataAnnotations;

namespace Sentaku.AuthServer.ViewModels;

public class SignupViewModel
{
  [Required]
  public string Username { get; set; }
  
  [Required]
  [EmailAddress]
  public string Email { get; set; }
  
  [Required]
  public string Name { get; set; }
  
  [Required]
  public string Surname { get; set; }
  
  [Required]
  [DataType(DataType.Date)]
  public DateTime Birthday { get; set; }
  
  [Required]
  [DataType(DataType.Password)]
  public string Password { get; set; }
  
  [Required]
  [DataType(DataType.Password)]
  [Compare("Password")]
  public string ConfirmPassword { get; set; }
  
  public string? ReturnTo { get; set; }
}
