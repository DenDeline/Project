﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Sentaku.SharedKernel.Models.VoteSession;

public class CreateVoteSessionRequest
{
  [Required]
  public DateTimeOffset StartDate { get; set; }
  
  [Required]
  public string Agenda { get; set; }
}
