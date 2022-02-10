using System;

namespace Sentaku.WebMVC.Models.Api.User
{
  public class GetUserResponse
  {
    public string Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Verified { get; set; }
    public string Birthday { get; set; }
  }
}
