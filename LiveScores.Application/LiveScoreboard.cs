using LiveScores.Application.Contracts;
using LiveScores.Domain.Entities;

namespace LiveScores.Application
{
    public class LiveScoreboard(IMatchStorage storage) : ILiveScoreboard
    {
        private readonly HashSet<string> _teamsPlaying = [];
        private readonly ReaderWriterLockSlim _lockSlim = new();

        public OperationResult<Guid?> AddMatch(string homeTeam, string awayTeam, DateTime started)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                if (_teamsPlaying.Contains(homeTeam) || _teamsPlaying.Contains(awayTeam))
                {
                    return new OperationResult<Guid?>(null, false,
                        new Dictionary<string, string> { { "TeamException", "Team already playing" } });
                }

                var newMatch = new Match(homeTeam, awayTeam, started);
                var result = storage.Add(newMatch);
                if (result.IsSuccess)
                {
                    _teamsPlaying.Add(homeTeam);
                    _teamsPlaying.Add(awayTeam);
                    return new OperationResult<Guid?>(newMatch.Id, true, null);
                }
                
                return new OperationResult<Guid?>(null, false, result.Errors);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public OperationResult<bool> UpdateScore(Guid matchId, byte newHomeTeamScore, byte newAwayTeamScore)
        {
            var getOperationResult = storage.Get(matchId);
            if (getOperationResult.IsSuccess)
            {
                getOperationResult.Data.UpdateScore(newHomeTeamScore, newAwayTeamScore);
                storage.Update(getOperationResult.Data);
                return new OperationResult<bool>(true, true, null);
            }
            return new OperationResult<bool>(false, false, getOperationResult.Errors);
        }

        public OperationResult<bool> FinishMatch(Guid matchId)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                var getOperationResult = storage.Get(matchId);
                if (getOperationResult.IsSuccess)
                {
                    storage.Delete(matchId);
                    _teamsPlaying.Remove(getOperationResult.Data.HomeTeam);
                    _teamsPlaying.Remove(getOperationResult.Data.AwayTeam);
                    return new OperationResult<bool>(true, true, null);
                }
                return new OperationResult<bool>(false, false, getOperationResult.Errors);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public OperationResult<MatchDto[]> GetLiveMatches()
        {
            var getAllResult = storage.GetAll();
            if (getAllResult.IsSuccess)
            {
                var matches = 
                    getAllResult.Data
                        .Select(x => new MatchDto(x.Id, x.HomeTeam, x.AwayTeam, x.HomeTeamScore, x.AwayTeamScore, x.DateStarted))
                        .OrderByDescending(x => x.AwayTeamScore + x.HomeTeamScore)
                        .ThenBy(x => x.Started)
                        .ToArray();
                return new OperationResult<MatchDto[]>(matches, true, null);
            }

            return new OperationResult<MatchDto[]>(null, false, getAllResult.Errors);
        }
    }
}
