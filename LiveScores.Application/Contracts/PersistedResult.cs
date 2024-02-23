namespace LiveScores.Application.Contracts;

public record PersistedResult<T>(T? Data, bool IsSuccess, IDictionary<string, string>? Errors);
