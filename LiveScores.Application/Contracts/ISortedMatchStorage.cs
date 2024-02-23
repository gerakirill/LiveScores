using LiveScores.Domain.Entities;

namespace LiveScores.Application.Contracts;

public interface ISortedMatchStorage
{
    PersistedResult<Match?> Get(Guid id);

    bool Add(Match match);

    bool Delete(Guid id);

    bool Update(Guid id, Action<Match> action);
}