using System;
using System.Collections.Generic;
using Sentaku.SharedKernel.Enums;

namespace Sentaku.SharedKernel.Models.User;

public class GetCurrentUserResponse
{
  public string Id { get; set; }
  public string Username { get; set; }
  public string Name { get; set; }
  public string Surname { get; set; }
  public string Email { get; set; }
  public DateTime CreatedAt { get; set; }
  public bool Verified { get; set; }
  public string Birthday { get; set; }
  public IEnumerable<string> Roles { get; set; }
  public Permissions Permissions { get; set; }
}
