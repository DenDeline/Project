using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.AuthServer.AuthServer.Models;
using Sentaku.AuthServer.Models.Oauth;
using Sentaku.AuthServer.ViewModels;
using Sentaku.Infrastructure.Data;

namespace Sentaku.AuthServer.Controllers;

public class OauthController : Controller
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public OauthController(
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
    }

    [HttpGet("/login")]
    public IActionResult Login([FromQuery] string returnTo)
    {
      var vm = new LoginViewModel();

      if (Url.IsLocalUrl(returnTo))
        vm.ReturnTo = returnTo;

      return View(vm);
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromForm] LoginViewModel request, CancellationToken cancellationToken)
    {
      var userIdRequest = await _userManager.Users.Where(_ =>
        _.NormalizedEmail == _userManager.NormalizeEmail(request.Login) ||
        _.NormalizedUserName == _userManager.NormalizeName(request.Login))
        .Select(_ => new { _.Id })
        .FirstOrDefaultAsync(cancellationToken);
      
      if (userIdRequest is null)
      {
        ModelState.AddModelError(nameof(request.Login), "User doesn't exist");
        return View(request);
      }

      var user = await _userManager.FindByIdAsync(userIdRequest.Id);
      
      if (user is null)
      {
        ModelState.AddModelError(nameof(request.Login), "User doesn't exist");
        return View(request);
      }
      
      var signInResult =  await _signInManager.PasswordSignInAsync(user, request.Password, true, false);

      if (signInResult.Succeeded && Url.IsLocalUrl(request.ReturnTo))
        return LocalRedirect(request.ReturnTo);

      return View(request);
    }
    
    [HttpGet("/signup")]
    public IActionResult Signup([FromQuery] string returnTo)
    {
      var vm = new SignupViewModel();

      if (Url.IsLocalUrl(returnTo))
        vm.ReturnTo = returnTo;

      return View(vm);
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost("/signup")]
    public async Task<IActionResult> Signup([FromForm] SignupViewModel request, CancellationToken cancellationToken)
    {
      var usernameExists = await _userManager.Users.AnyAsync(_ => _.NormalizedUserName == _userManager.NormalizeName(request.Username), cancellationToken);

      if (usernameExists)
      {
        ModelState.AddModelError(nameof(request.Username), "Login has already taken");
        return View(request);
      }

      var emailExists = await _userManager.Users.AnyAsync(_ => _.NormalizedEmail == _userManager.NormalizeEmail(request.Email), cancellationToken);
      
      if (emailExists)
      {
        ModelState.AddModelError(nameof(request.Email), "Email has already taken");
        return View(request);
      }

      var user = new AppUser(request.Username)
      {
        Email = request.Email,
        LanguageId = 1
      };
      
      user.UpdateProfileInfo(request.Name, request.Surname);
      user.UpdateBirthday(request.Birthday);
      
      var createUserResult =  await _userManager.CreateAsync(user, request.Password);

      if (createUserResult.Succeeded)
      {
        var userid = await _userManager.GetUserIdAsync(user);
        user.Id = userid;
        
        await _signInManager.SignInAsync(user, true);
        
        if (Url.IsLocalUrl(request.ReturnTo))
          return LocalRedirect(request.ReturnTo);
      }

      return View();
    }
    
    [HttpGet("/login/oauth2/authorize")]
    public async Task<IActionResult> Authorize(
      [FromQuery] GetAuthorizationRequest request,
      [FromServices] IDataProtectionProvider provider)
    {
      var authResult = await HttpContext.AuthenticateAsync();
      
      if (!authResult.Succeeded)
        return RedirectToAction("Login", new { ReturnTo = Url.Action("Authorize") + Request.QueryString });

      var userId = _userManager.GetUserId(authResult.Principal);

      var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == request.ClientId);

      // TODO: Separate class for validation required params 
      if (client is null)
        return ResponseHelper.InvalidClient(request.RedirectUri, state: request.State);

      if (!client.RedirectUris.Contains(request.RedirectUri))
        return ResponseHelper.InvalidRequest(request.RedirectUri, state: request.State);

      if (AuthServerConfig.SupportedResponseTypes.All(_ => _ != request.ResponseType))
        return ResponseHelper.UnsupportedGrantType(request.RedirectUri, state: request.State);

      var codeToken = new CodeToken
      {
        ClientId = request.ClientId,
        RedirectUri = request.RedirectUri,
        UserId = userId,
        CodeChallenge = request.CodeChallenge,
        CodeChallengeMethod = request.CodeChallengeMethod
      };

      var jsonCodeToken = JsonConvert.SerializeObject(codeToken);

      var protector =  provider
        .CreateProtector("AuthServer.Oauth2.SecureCodeToken")
        .ToTimeLimitedDataProtector();
      
      var encodedCodeToken = protector.Protect(jsonCodeToken, TimeSpan.FromMinutes(5));

      // TODO: Create separate class for building redirectUrl
      var queryBuilder = new QueryBuilder
      {
        {"code", encodedCodeToken},
        {"state", request.State}
      };

      var redirectUrl = $"{client.RedirectUris.FirstOrDefault()}{queryBuilder}";
      return Redirect(redirectUrl);
    }


    [HttpPost("/login/oauth2/token")]
    public async Task<IActionResult> GetAccessTokenAsync(
      [FromForm] GetAccessTokenRequest request,
      [FromServices] IIdentityTokenClaimService tokenService,
      [FromServices] IDataProtectionProvider provider,
      CancellationToken cancellationToken)
    {
      if (AuthServerConfig.SupportedGrantTypes.All(_ => _ != request.GrantType))
        return ResponseHelper.UnsupportedGrantType(request.RedirectUri);

      var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == request.ClientId);
      
      if (client is null)
        return ResponseHelper.InvalidClient(request.RedirectUri);

      var protector =  provider
        .CreateProtector("AuthServer.Oauth2.SecureCodeToken")
        .ToTimeLimitedDataProtector();

      string decodedCodeToken;
      
      try
      { 
        decodedCodeToken = protector.Unprotect(request.Code);
      }
      catch (CryptographicException e)
      {
        return ResponseHelper.InvalidGrant(request.RedirectUri, e.Message);
      }
      
      var codeToken = JsonConvert.DeserializeObject<CodeToken?>(decodedCodeToken);

      if (codeToken is null)
        return ResponseHelper.InvalidGrant(request.RedirectUri);

      var codeTokenValidationResult = codeToken.Validate(request.ClientId, request.RedirectUri, request.CodeVerifier);

      if (!codeTokenValidationResult.IsValid)
        return ResponseHelper.ErrorResponse(request.RedirectUri, codeTokenValidationResult.Error, codeTokenValidationResult.ErrorDescription);

      var tokenResult = await tokenService.GetTokenAsync(
        codeToken.UserId, 
        "https://localhost:7045", 
        "https://localhost:5001",
        cancellationToken);

      if (tokenResult.IsSuccess)
        return Ok(tokenResult.Value);
      
      return ResponseHelper.ResourceOwnerNotFound(request.RedirectUri);
    }
  }
