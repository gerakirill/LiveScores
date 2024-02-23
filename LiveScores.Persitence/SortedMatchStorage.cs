using LiveScores.Application.Contracts;
using LiveScores.Domain.Entities;

namespace LiveScores.Persistence;

public class SortedMatchStorage : ISortedMatchStorage
{
    public PersistedResult<Match?> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool Add(Match match)
    {
        throw new NotImplementedException();
    }

    public bool Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool Update(Guid id, Action<Match> action)
    {
        throw new NotImplementedException();
    }
}