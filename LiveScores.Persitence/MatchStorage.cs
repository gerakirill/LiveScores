using System.Collections.Concurrent;
using LiveScores.Application.Contracts;
using LiveScores.Domain.Entities;

namespace LiveScores.Persistence;

public class MatchStorage : IMatchStorage
{
    private readonly ConcurrentDictionary<Guid, Match> _storage = new();

    public OperationResult<Match?> Get(Guid id)
    {
        bool operationResult = _storage.TryGetValue(id, out Match match);
        if (!operationResult)
            return new OperationResult<Match?>(null, false, new Dictionary<string, string> { { "KeyNotFound", "Match with specified key not found" } });
        return new OperationResult<Match?>(match, true, null);
    }

    public OperationResult<bool> Add(Match match)
    {
        bool operationResult = _storage.TryAdd(match.Id, match);

        if (!operationResult)
            return new OperationResult<bool>(false, false, new Dictionary<string, string> { { "KeyExists", "Match with specified key already exists" } });
        return new OperationResult<bool>(true, true, null);
    }
    public OperationResult<bool> Delete(Guid id)
    {
        bool operationResult = _storage.Remove(id, out _);

        if (!operationResult)
            return new OperationResult<bool>(false, false, new Dictionary<string, string> { { "KeyNotFound", "Match with specified key not found" } });
        return new OperationResult<bool>(true, true, null);
    }

    public OperationResult<bool> Update(Match match)
    {
        bool getOperationResult = _storage.TryGetValue(match.Id, out Match existingMatch);

        if (!getOperationResult)
            return new OperationResult<bool>(false, false, new Dictionary<string, string> { { "KeyNotFound", "Match with specified key not found" } });

        bool updateOperationResult = _storage.TryUpdate(match.Id, match, existingMatch);
        if (!updateOperationResult)
            return new OperationResult<bool>(false, false, new Dictionary<string, string> { { "UpdateFailed", "Update operation failed" } });

        return new OperationResult<bool>(true, true, null);
    }

    public OperationResult<Match[]> GetAll()
    {
        var result = _storage.Values.ToArray();
        return new OperationResult<Match[]>(result, true, null);
    }
}