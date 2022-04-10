using System;

namespace Sentaku.SharedKernel.Models;

public class UserDto
{
  public string Id { get; set; }
  public string Username { get; set; }
  public string Name { get; set; }
  public string Surname { get; set; }
  public bool Verified { get; set; }
  public DateTime CreatedAt { get; set; }
}
