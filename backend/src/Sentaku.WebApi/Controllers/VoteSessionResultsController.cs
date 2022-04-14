using System;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using IronPdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Interfaces;
using Sentaku.WebApi.Authorization.PermissionsAuthorization;
using Microsoft.AspNetCore.Http;
using Sentaku.SharedKernel.Enums;

namespace Sentaku.WebApi.Controllers;

[ApiController]
[Route("/sessions/{sessionId:guid}/results")]
public class VoteSessionResultsController: ControllerBase
{
  private readonly UserManager<AppUser> _userManager;
  private readonly IReadRepository<VoteSession> _voteSessionReadRepository;
  private readonly IPermissionsService _permissionsService;

  public VoteSessionResultsController(
    UserManager<AppUser> userManager,
    IReadRepository<VoteSession> voteSessionReadRepository,
    IPermissionsService permissionsService)
  {
    _userManager = userManager;
    _voteSessionReadRepository = voteSessionReadRepository;
    _permissionsService = permissionsService;
  }
  
  [HttpGet]
  [RequirePermissions(Permissions.ViewVotingSessions)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesDefaultResponseType]
  public async Task<ActionResult> GetResultsBySessionId(
    [FromRoute] Guid sessionId,
    CancellationToken cancellationToken)
  {
    var userId = _userManager.GetUserId(User);

    if (userId is null)
      return Unauthorized();
    
    var stateSpec = new VoteSessionStateByIdSpec(sessionId);
    
    var state = await _voteSessionReadRepository.GetBySpecAsync(stateSpec, cancellationToken);

    if (state is null)
      return NotFound();

    if (state.Equals(SessionState.Pending) || state.Equals(SessionState.Active))
      return BadRequest();

    if (state.Equals(SessionState.Closed))
    {
      var result = await _permissionsService.ValidatePermissionsAsync(User, Permissions.ManageVotingSessions, cancellationToken);
      if (!result.Succeeded)
        return BadRequest();
    }

    var voteSessionSpec = new VoteSessionResultsByIdSpec(sessionId);

    var results = await _voteSessionReadRepository.GetBySpecAsync(voteSessionSpec, cancellationToken);

    if (results is null)
      return NotFound();

    return Ok(results);
  }
  
  [HttpPut("export")]
  [RequirePermissions(Permissions.ViewVotingSessions)]
  public async Task<ActionResult> ExportResultsBySessionId(
    [FromRoute] Guid sessionId,
    CancellationToken cancellationToken)
  {
    var userId = _userManager.GetUserId(User);

    if (userId is null)
      return Unauthorized();
    
    var stateSpec = new VoteSessionStateByIdSpec(sessionId);
    
    var state = await _voteSessionReadRepository.GetBySpecAsync(stateSpec, cancellationToken);

    if (state is null)
      return NotFound();

    if (state.Equals(SessionState.Pending) || state.Equals(SessionState.Active))
      return BadRequest();

    if (state.Equals(SessionState.Closed))
    {
      var result = await _permissionsService.ValidatePermissionsAsync(User, Permissions.ManageVotingSessions, cancellationToken);
      if (!result.Succeeded)
        return BadRequest();
    }

    var voteSessionSpec = new VoteSessionResultsByIdSpec(sessionId);

    var results = await _voteSessionReadRepository.GetBySpecAsync(voteSessionSpec, cancellationToken);

    if (results is null)
      return NotFound();
    
    var renderer = new ChromePdfRenderer();
    using var pdf = await renderer.RenderHtmlAsPdfAsync(@$"
<!DOCTYPE html>
<html>

<head>
	<title>Parcel Sandbox</title>
	<meta charset='UTF-8' />
</head>

<body>
	<div>
    <h1>Voting {results.ActivatedOn:d}</h1>
    <div>
      <h2>Agenda</h2>
      <div>{results.Agenda}</div>
    </div>
    <div>
      <h2>Questions</h2>
      <div>
        {string.Join('\n', results.Questions.Select(questionWithResults => $@"
          <h3>{questionWithResults.Summary}</h3>
          <p>{questionWithResults.Details}</p>
          <p>Results: {string.Join(' ', questionWithResults.Results.Select(result => $"<span>{result.Type}</span> - <span>{result.Count}</span>"))}</p>
        "))}
      </div>
    </div>
  </div>
</body>

</html>
");

    Response.ContentLength = pdf.BinaryData.Length;
    Response.Headers.Add("Content-Disposition", "attachment; filename=results_" + sessionId + ".pdf");

    return File(pdf.BinaryData,MediaTypeNames.Application.Pdf);
  }
}
