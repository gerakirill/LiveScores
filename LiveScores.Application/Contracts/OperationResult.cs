namespace LiveScores.Application.Contracts;

public record OperationResult<T>(T? Data, bool IsSuccess, IDictionary<string, string>? Errors);
