using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.AuthServer.AuthServer;
using Sentaku.AuthServer.AuthServer.Models;
using Sentaku.AuthServer.Models.Oauth;
using Sentaku.AuthServer.ViewModels;
using Sentaku.Infrastructure.Data;

namespace Sentaku.AuthServer.Controllers;

public class OauthController : Controller
  {
    private readonly UserManager<AppUser> _userManager;

    public OauthController(UserManager<AppUser> userManager)
    {
      _userManager = userManager;
    }

    [HttpGet("/oauth2/authorize")]
    public IActionResult Authorize([FromQuery] GetAuthorizationRequest request)
    {
      var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == request.ClientId);

      // TODO: Separate class for validation required params 
      if (client is null)
      {
        var queryBuilder = new QueryBuilder
        {
          {"error", AuthServerConstants.ErrorResponseTypes.InvalidClient},
          {"state", request.State}
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      if (!client.RedirectUris.Contains(request.RedirectUri))
      {
        var queryBuilder = new QueryBuilder
        {
          {"error", AuthServerConstants.ErrorResponseTypes.InvalidRequest},
          {"state", request.State}
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      if (AuthServerConfig.SupportedResponseTypes.All(_ => _ != request.ResponseType))
      {
        var queryBuilder = new QueryBuilder
        {
          {"error", AuthServerConstants.ErrorResponseTypes.UnsupportedGrantType},
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
    public async Task<IActionResult> SignInCode([FromForm] AuthorizeViewModel vm)
    {
      var user = (await _userManager.FindByEmailAsync(vm.Login)) ?? (await _userManager.FindByNameAsync(vm.Login));

      if (user is null)
      {
        // TODO: Add validation error and append query params
        ModelState.AddModelError(vm.Login, "");
        return View("Authorize", vm);
      }

      var isPasswordValid = await _userManager.CheckPasswordAsync(user, vm.Password);

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

      var encodedCodeToken = await StringEncryption.AesEncryptAsync(jsonCodeToken, client.ClientSecret);

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
    public async Task<IActionResult> GetAccessTokenAsync(
      [FromForm] GetAccessTokenRequest request,
      [FromServices] IIdentityTokenClaimService tokenService,
      CancellationToken cancellationToken)
    {
      if (AuthServerConfig.SupportedGrantTypes.All(_ => _ != request.GrantType))
      {
        var queryBuilder = new QueryBuilder
        {
          { "error", AuthServerConstants.ErrorResponseTypes.UnsupportedGrantType }
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == request.ClientId);
      
      if (client is null)
      {
        var queryBuilder = new QueryBuilder
        {
          { "error", AuthServerConstants.ErrorResponseTypes.InvalidClient }
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      var decodedCodeToken = await StringEncryption.AesDecryptAsync(request.Code, client.ClientSecret);
      var codeToken = JsonConvert.DeserializeObject<CodeToken?>(decodedCodeToken);

      if (codeToken is null)
      {
        var queryBuilder = new QueryBuilder
        {
          { "error", AuthServerConstants.ErrorResponseTypes.InvalidGrant }
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      var codeTokenValidationResult = codeToken.Validate(request.ClientId, request.RedirectUri, request.CodeVerifier);

      if (!codeTokenValidationResult.IsValid)
      {
        var queryBuilder = new QueryBuilder
        {
          {"error", codeTokenValidationResult.Error},
          {"error_description", codeTokenValidationResult.ErrorDescription}
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      var user = await _userManager.FindByIdAsync(codeToken.UserId);

      if (user is null)
      {
        var queryBuilder = new QueryBuilder
        {
          { "error", AuthServerConstants.ErrorResponseTypes.InvalidRequest }
        };
        return Redirect($"{request.RedirectUri}{queryBuilder}");
      }

      var token = await tokenService.GetTokenAsync(
        user.UserName, 
        "https://localhost:7045", 
        "https://localhost:5001");

      return Ok(new
      {
        access_token = token,
        token_type = "Bearer",
        expires_in = 3600
      });
    }
  }
