using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;
using Sentaku.SharedKernel.Interfaces;
using Sentaku.SharedKernel.Models.VoteSessionQuestions;
using Microsoft.AspNetCore.Http;

namespace Sentaku.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/sessions/{sessionId:guid}/questions")]
public class QuestionsController: ControllerBase
{
  private readonly IReadRepository<VoteSession> _voteSessionReadRepository;
  private readonly IRepository<VoteSession> _voteSessionRepository;

  public QuestionsController(
    IReadRepository<VoteSession> voteSessionReadRepository,
    IRepository<VoteSession> voteSessionRepository)
  {
    _voteSessionReadRepository = voteSessionReadRepository;
    _voteSessionRepository = voteSessionRepository;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Question>>> ListQuestions(
    [FromRoute] Guid sessionId,
    CancellationToken cancellationToken)
  {
    var spec = new QuestionsByVoteSessionIdSpec(sessionId);

    var questions =  await _voteSessionReadRepository.GetBySpecAsync(spec, cancellationToken);

    return questions is null ? NotFound() : Ok(questions);
  }
  
  [HttpGet("{questionIndex:int}")]
  public async Task<ActionResult<Question>> GetQuestionByIndex(
    [FromRoute] Guid sessionId,
    [FromRoute] int questionIndex,
    CancellationToken cancellationToken)
  {
    var spec = new QuestionByVoteSessionIdAndIndexSpec(sessionId, questionIndex);

    var question =  await _voteSessionReadRepository.GetBySpecAsync(spec, cancellationToken);

    return question is null ? NotFound() : Ok(question);
  }
  
  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesDefaultResponseType]
  public async Task<ActionResult> CreateQuestion(
    [FromRoute] Guid sessionId,
    [FromBody] CreateQuestionByVoteSessionIdRequest request,
    CancellationToken cancellationToken = default)
  {
    var session =  await _voteSessionRepository.GetByIdAsync(sessionId, cancellationToken);

    if (session is null)
      return NotFound();
    
    var question = session.AddQuestion(request.Summary, request.Description);

    if (question is null)
      return BadRequest();
    
    await _voteSessionRepository.SaveChangesAsync(cancellationToken);

    return CreatedAtAction(nameof(GetQuestionByIndex), new { sessionId, questionIndex = question.Index }, question);
  }
  
  [HttpPut("{questionIndex:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesDefaultResponseType]
  public async Task<ActionResult<Question>> UpdateQuestionByIndex(
    [FromRoute] Guid sessionId,
    [FromRoute] int questionIndex,
    [FromBody] UpdateQuestionByIndexRequest request,
    CancellationToken cancellationToken)
  {
    var spec = new VoteSessionByIdIncludeQuestionsSpec(sessionId);

    var session =  await _voteSessionRepository.GetBySpecAsync(spec, cancellationToken);

    if (session is null)
      return NotFound();

    var question = session.GetQuestionByIndex(questionIndex);
    
    if (question is null)
      return NotFound();

    question.UpdatePrimaryInfo(request.Summary, request.Desciption);

    await _voteSessionRepository.SaveChangesAsync(cancellationToken);

    return NoContent();
  }
  
  [HttpDelete("{questionIndex:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesDefaultResponseType]
  public async Task<ActionResult<Question>> DeleteQuestionByIndex(
    [FromRoute] Guid sessionId,
    [FromRoute] int questionIndex,
    CancellationToken cancellationToken)
  {
    var spec = new VoteSessionByIdIncludeQuestionsSpec(sessionId);

    var session =  await _voteSessionRepository.GetBySpecAsync(spec, cancellationToken);

    if (session is null)
      return NotFound();

    if (session.RemoveQuestionByIndex(questionIndex))
    {
      await _voteSessionRepository.SaveChangesAsync(cancellationToken);
      return NoContent();
    };
    
    return BadRequest();
  }
}
