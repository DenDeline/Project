using System;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Ardalis.Result.AspNetCore;

public static class ResultExtensions
  {
    public static ActionResult ToActionResult(
      this Ardalis.Result.Result result,
      ControllerBase controller)
    {
      return (ActionResult) controller.ToActionResult((IResult) result);
    }

    public static ActionResult ToActionResult(
      this ControllerBase controller,
      Ardalis.Result.Result result)
    {
      return (ActionResult) controller.ToActionResult((IResult) result);
    }

    public static ActionResult<TDestination> MapToActionResult<TSource, TDestination>(
      this IMapper mapper,
      Result<TSource> source,
      ControllerBase controller)
    {
      return (ActionResult<TDestination>) controller.MapToActionResult<TDestination>((IResult) source, mapper);
    }

    private static ActionResult MapToActionResult<TDestination>(
      this ControllerBase controller,
      IResult result,
      IMapper mapper)
    {
      switch (result.Status)
      {
        case ResultStatus.Ok:
          return (ActionResult)controller.Ok(mapper.Map<TDestination>(result.GetValue()));
        case ResultStatus.Error:
          return ResultExtensions.UnprocessableEntity(controller, result);
        case ResultStatus.Forbidden:
          return (ActionResult) controller.Forbid();
        case ResultStatus.Unauthorized:
          return (ActionResult) controller.Unauthorized();
        case ResultStatus.Invalid:
          return ResultExtensions.BadRequest(controller, result);
        case ResultStatus.NotFound:
          return (ActionResult) controller.NotFound();
        default:
          throw new NotSupportedException(string.Format("Result {0} conversion is not supported.", (object) result.Status));
      }
    }
    
    private static ActionResult ToActionResult(
      this ControllerBase controller,
      IResult result)
    {
      switch (result.Status)
      {
        case ResultStatus.Ok:
          return (ActionResult)controller.NoContent();
        case ResultStatus.Error:
          return ResultExtensions.UnprocessableEntity(controller, result);
        case ResultStatus.Forbidden:
          return (ActionResult) controller.Forbid();
        case ResultStatus.Unauthorized:
          return (ActionResult) controller.Unauthorized();
        case ResultStatus.Invalid:
          return ResultExtensions.BadRequest(controller, result);
        case ResultStatus.NotFound:
          return (ActionResult) controller.NotFound();
        default:
          throw new NotSupportedException(string.Format("Result {0} conversion is not supported.", (object) result.Status));
      }
    }

    private static ActionResult BadRequest(ControllerBase controller, IResult result)
    {
      foreach (ValidationError validationError in result.ValidationErrors)
        controller.ModelState.AddModelError(validationError.Identifier, validationError.ErrorMessage);
      return (ActionResult) controller.BadRequest(controller.ModelState);
    }

    private static ActionResult UnprocessableEntity(
      ControllerBase controller,
      IResult result)
    {
      StringBuilder stringBuilder = new StringBuilder("Next error(s) occured:");
      foreach (string error in result.Errors)
        stringBuilder.Append("* ").Append(error).AppendLine();
      return (ActionResult) controller.UnprocessableEntity((object) new ProblemDetails()
      {
        Title = "Something went wrong.",
        Detail = stringBuilder.ToString()
      });
    }
  }
