using LiveScores.Domain.Entities;

namespace LiveScores.Application.Contracts;

public interface IMatchStorage
{
    OperationResult<Match?> Get(Guid id);

    OperationResult<bool> Add(Match match);

    OperationResult<bool> Delete(Guid id);

    OperationResult<bool> Update(Match match);

    OperationResult<Match[]> GetAll();
}