using LiveScores.Application.Contracts;

namespace LiveScores.Application
{
    public class LiveScoreboard : ILiveScoreboard
    {
        public OperationResult<Guid?> AddMatch(string homeTeam, string awayTeam, DateTime started)
        {
            throw new NotImplementedException();
        }

        public OperationResult<bool> UpdateScore(Guid matchId, byte newHomeTeamScore, byte newAwayTeamScore)
        {
            throw new NotImplementedException();
        }

        public OperationResult<bool> FinishMatch(Guid matchId)
        {
            throw new NotImplementedException();
        }

        public OperationResult<MatchDto[]> GetLiveMatches()
        {
            throw new NotImplementedException();
        }
    }
}
