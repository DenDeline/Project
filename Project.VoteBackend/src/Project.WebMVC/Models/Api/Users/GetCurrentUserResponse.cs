﻿using System;

namespace Project.WebMVC.Models.Api.Users
{
  public class GetCurrentUserResponse
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Verified { get; set; }
    public string ProfileImageUrl { get; set; }
    public string Birthday { get; set; }
  }
}