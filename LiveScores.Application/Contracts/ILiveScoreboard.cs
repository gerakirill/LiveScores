namespace LiveScores.Application.Contracts;

public interface ILiveScoreboard
{
    OperationResult<Guid?> AddMatch(string homeTeam, string awayTeam, DateTime started);

    OperationResult<bool> UpdateScore(Guid matchId, byte newHomeTeamScore, byte newAwayTeamScore);

    OperationResult<bool> FinishMatch(Guid matchId);

    OperationResult<MatchDto[]> GetLiveMatches();
}