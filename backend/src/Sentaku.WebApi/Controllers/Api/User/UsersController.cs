using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sentaku.Infrastructure.Data;
using Sentaku.WebApi.Models.Api.User;

namespace Sentaku.WebApi.Controllers.Api.User
{
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public UsersController(
        UserManager<AppUser> userManager,
        IMapper mapper)
    {
      _userManager = userManager;
      _mapper = mapper;
    }

    [HttpGet("/api/users")]
    public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetAllUsers(CancellationToken ctsToken)
    {
      var users = await _userManager.Users.ToListAsync(ctsToken);
      return Ok(_mapper.Map<IEnumerable<GetUserResponse>>(users));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("/api/user")]
    public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUser()
    {
      if (User.Identity?.Name is null)
      {
        return Forbid();
      }

      var user = await _userManager.FindByNameAsync(User.Identity.Name);

      var dto = _mapper.Map<GetCurrentUserResponse>(user);

      return Ok(dto);
    }

    [HttpGet("/api/users/{username}")]
    public async Task<ActionResult<GetUserResponse>> GetUserByName([FromRoute] string username)
    {
      var user = await _userManager.FindByNameAsync(username);

      if (user is null)
      {
        return NotFound();
      }

      var dto = _mapper.Map<GetUserResponse>(user);

      return Ok(dto);
    }
  }
}
