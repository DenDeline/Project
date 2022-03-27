using System;

namespace Sentaku.WebApi.Models.User;

public class GetUserWithPrivateInfoResponse
{
  public string Id { get; set; }
  public string Username { get; set; }
  public string Name { get; set; }
  public string Surname { get; set; }
  public bool Verified { get; set; }
  public DateTime CreatedAt { get; set; }
}
