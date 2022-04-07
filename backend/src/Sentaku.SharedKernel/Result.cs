using System;
using System.Collections.Generic;

namespace Ardalis.Result;

public class Result: IResult
{
  public Result() { }

  private Result(ResultStatus status) => this.Status = status;

  public ResultStatus Status { get; }
  public IEnumerable<string> Errors { get; private init; }
  public List<ValidationError> ValidationErrors { get; private init; }
  
  public Type? ValueType { get; }
  
  public bool IsSuccess => this.Status == ResultStatus.Ok;
  public string SuccessMessage { get; private init; } = string.Empty;

  public object? GetValue() => null;

  public static Result Success() => new();

  public static Result Success(string successMessage) => new()
  {
    SuccessMessage = successMessage
  };

  public static Result Error(params string[] errorMessages) => new(ResultStatus.Error)
  {
    Errors = errorMessages
  };

  public static Result Invalid(List<ValidationError> validationErrors) => new(ResultStatus.Invalid)
  {
    ValidationErrors = validationErrors
  };

  public static Result NotFound() => new(ResultStatus.NotFound);

  public static Result Forbidden() => new(ResultStatus.Forbidden);

  public static Result Unauthorized() => new(ResultStatus.Unauthorized);
}
