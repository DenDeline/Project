using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Data;
using Project.WebMVC.AuthServer.Extensions;
using Project.WebMVC.AuthServer.Models;
using Project.WebMVC.Models.Oauth;
using Project.WebMVC.ViewModels;

namespace Project.WebMVC.Controllers
{
  public class OauthController: Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public OauthController(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }
    
    [HttpGet("/oauth2/authorize")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Authorize([FromQuery] GetAuthorizationRequest request)
    {
      var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == request.ClientId);

      // TODO: Separate class for validation required params 
      if (client is null)
      {
       return BadRequest();
      }

      if (!client.RedirectUris.Contains(request.RedirectUri))
      {
        //TODO: Add error validation constant: invalid_request
        var queryBuilder = new QueryBuilder
        {
          {"error", "invalid_request"},
          {"state", request.State}
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      if (AuthServerConfig.SupportedResponseTypes.All(_ => _ != request.ResponseType))
      {
        //TODO: Add error validation constant: unsupported_response_type
        var queryBuilder = new QueryBuilder
        {
        {"error", "unsupported_response_type"},
        {"state", request.State}
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      var vm = new AuthorizeViewModel
      {
        ClientId = request.ClientId,
        RedirectUri = request.RedirectUri,
        State = request.State,
        CodeChallenge = request.CodeChallenge,
        CodeChallengeMethod = request.CodeChallengeMethod
      };

      return View(vm);
    }

    [HttpPost("/oauth2/signin-code")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> SignInCode([FromForm] AuthorizeViewModel vm)
    {
      var user = (await _userManager.FindByEmailAsync(vm.Login)) ?? (await  _userManager.FindByNameAsync(vm.Login));

      if (user is null)
      {
        // TODO: Add validation error and append query params
        ModelState.AddModelError(vm.Login, "");
        return View("Authorize", vm);
      }

      var isPasswordValid =  await _userManager.CheckPasswordAsync(user, vm.Password);

      if (!isPasswordValid)
      {
        // TODO: Add validation error and append query params
        ModelState.AddModelError(vm.Password, "");
        return View("Authorize", vm);
      }
      
      // TODO: Move out code token generation
      var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == vm.ClientId);

      if (client is null)
      {
        // TODO: Add validation error and append query params
        ModelState.AddModelError("", "");
        return View("Authorize", vm);
      }

      var codeToken = new CodeToken
      {
        ClientId = vm.ClientId,
        RedirectUri = vm.RedirectUri,
        UserId = user.Id,
        CodeChallenge = vm.CodeChallenge,
        CodeChallengeMethod = vm.CodeChallengeMethod
      };

      var jsonCodeToken = JsonConvert.SerializeObject(codeToken);

      var encodedCodeToken = jsonCodeToken.AesEncrypt(client.ClientSecret);

      // TODO: Create separate class for building redirectUrl
      var queryBuilder = new QueryBuilder
      {
        {"code", encodedCodeToken}, 
        {"state", vm.State}
      };

      var redirectUrl = $"{client.RedirectUris.FirstOrDefault()}{queryBuilder}";
      return Redirect(redirectUrl);
    }


    [HttpPost("/oauth2/token")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetAccessTokenAsync(
      [FromForm] GetAccessTokenRequest request,
      [FromServices] IIdentityTokenClaimService tokenService,
      CancellationToken cts = new CancellationToken())
    {
      if (AuthServerConfig.SupportedGrantTypes.All(_ => _ != request.GrantType))
      {
        // TODO: Add validation error
        return BadRequest();
      }

      var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == request.ClientId);
      if (client is null)
      {
        // TODO: Add validation error
        return BadRequest();
      }

      var decodedCodeToken = request.Code.AesDecrypt(client.ClientSecret);
      var codeToken = JsonConvert.DeserializeObject<CodeToken>(decodedCodeToken);

      if (codeToken is null || !codeToken.IsValid( request.ClientId, request.RedirectUri, request.CodeVerifier))
      {
        return BadRequest();
      }


      var user = await _userManager.FindByIdAsync(codeToken.UserId);

      if (user is null)
      {
        return NotFound();
      }

      var token = await tokenService.GetTokenAsync(user.UserName);

      return Ok(new
      {
        access_token = token,
        token_type = "Bearer",
        expires_in = 3600
      });
    }
  }
}
