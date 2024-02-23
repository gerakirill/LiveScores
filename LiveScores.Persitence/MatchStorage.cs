using System.Collections.Concurrent;
using LiveScores.Application.Contracts;
using LiveScores.Domain.Entities;

namespace LiveScores.Persistence;

public class MatchStorage : IMatchStorage
{
    private readonly ConcurrentDictionary<Guid, Match> _storage = new();


    public PersistedResult<Match?> Get(Guid id)
    {
        bool operationResult = _storage.TryGetValue(id, out Match match);
        if (!operationResult)
            return new PersistedResult<Match?>(null, false, new Dictionary<string, string> { { "KeyNotFound", "Match with specified key not found" } });
        return new PersistedResult<Match?>(match, true, null);
    }

    public PersistedResult<bool> Add(Match match)
    {
        bool operationResult = _storage.TryAdd(match.Id, match);

        if (!operationResult)
            return new PersistedResult<bool>(false, false, new Dictionary<string, string> { { "KeyExists", "Match with specified key already exists" } });
        return new PersistedResult<bool>(true, true, null);
    }
    public PersistedResult<bool> Delete(Guid id)
    {
        bool operationResult = _storage.Remove(id, out _);

        if (!operationResult)
            return new PersistedResult<bool>(false, false, new Dictionary<string, string> { { "KeyNotFound", "Match with specified key not found" } });
        return new PersistedResult<bool>(true, true, null);
    }

    public PersistedResult<bool> Update(Match match)
    {
        bool getOperationResult = _storage.TryGetValue(match.Id, out Match existingMatch);

        if (!getOperationResult)
            return new PersistedResult<bool>(false, false, new Dictionary<string, string> { { "KeyNotFound", "Match with specified key not found" } });

        bool updateOperationResult = _storage.TryUpdate(match.Id, match, existingMatch);
        if (!updateOperationResult)
            return new PersistedResult<bool>(false, false, new Dictionary<string, string> { { "UpdateFailed", "Update operation failed" } });

        return new PersistedResult<bool>(true, true, null);
    }
}