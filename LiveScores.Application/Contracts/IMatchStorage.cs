using LiveScores.Domain.Entities;

namespace LiveScores.Application.Contracts;

public interface IMatchStorage
{
    PersistedResult<Match?> Get(Guid id);

    PersistedResult<bool> Add(Match match);

    PersistedResult<bool> Delete(Guid id);

    PersistedResult<bool> Update(Match match);
}