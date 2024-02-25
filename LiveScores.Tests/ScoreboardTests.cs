using LiveScores.Application.Contracts;
using LiveScores.Persistence;
using System.Collections.Concurrent;
using LiveScores.Application;
using Xunit;

namespace LiveScores.Tests
{
    public class ScoreboardTests
    {
        [Fact]
        public void AddMatch_ValidMatch_ReturnsGuid()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            // act
            OperationResult<Guid?> result = scoreBoard.AddMatch("team1", "team2", DateTime.Now);

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Null(result.Errors);
        }

        [Theory]
        [InlineData("team1", "team2")]
        [InlineData("team3", "team2")]
        [InlineData("team1", "team3")]
        public void AddMatch_DuplicateTeams_ReturnsError(string homeTeam, string awayTeam)
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            // act
            OperationResult<Guid?> result = scoreBoard.AddMatch("team1", "team2", DateTime.Now);
            OperationResult<Guid?> result2 = scoreBoard.AddMatch(homeTeam, awayTeam, DateTime.Now);

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Null(result.Errors);
            Assert.False(result2.IsSuccess);
            Assert.Null(result2.Data);
            Assert.NotNull(result2.Errors);
        }

        [Fact]
        public void AddMatch_ConcurrentWrite_ReturnsResult()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            var matchesToAdd = new List<Tuple<string, string, DateTime>>();
            
            for (int i = 0; i < 1000; i++)
            {
                matchesToAdd.Add(new ($"home{i}", $"away{i}", DateTime.Now));
            }

            ConcurrentBag<bool> results = [];
            // act
            Parallel.ForEach(matchesToAdd, match =>
            {
                var result = scoreBoard.AddMatch(match.Item1, match.Item2, match.Item3);
                results.Add(result.IsSuccess);
            });

            // assert
            Assert.Equal(results.Count, 1000);
            Assert.All(results, Assert.True);
        }

        [Fact]
        public void UpdateScore_ValidScore_ReturnsTrue()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());
            byte expectedHomeTeamScore = 1;
            byte expectedAwayTeamScore = 2;

            // act
            OperationResult<Guid?> addResult = scoreBoard.AddMatch("team1", "team2", DateTime.Now);
            OperationResult<bool> updateScoreResult = scoreBoard.UpdateScore(addResult.Data.Value, expectedHomeTeamScore, expectedAwayTeamScore);
            OperationResult<MatchDto[]> getMatchesResult = scoreBoard.GetLiveMatches();

            // assert
            Assert.True(updateScoreResult.IsSuccess);
            Assert.True(updateScoreResult.Data);
            Assert.Null(updateScoreResult.Errors);
            Assert.True(getMatchesResult.IsSuccess);
            Assert.Equal(getMatchesResult.Data.First().HomeTeamScore, expectedHomeTeamScore);
            Assert.Equal(getMatchesResult.Data.First().AwayTeamScore, expectedAwayTeamScore);
        }

        [Fact]
        public void UpdateScore_NotExistingMatch_ReturnsFalse()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            // act
            OperationResult<bool> updateScoreResult = 
                scoreBoard
                    .UpdateScore(Guid.NewGuid(), 1, 0);

            // assert
            Assert.False(updateScoreResult.IsSuccess);
            Assert.False(updateScoreResult.Data);
            Assert.NotNull(updateScoreResult.Errors);
        }

        [Fact]
        public void FinishMatch_ExistingMatch_ReturnsTrue()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            // act
            OperationResult<Guid?> addResult = scoreBoard.AddMatch("team1", "team2", DateTime.Now);
            OperationResult<bool> finishMatchResult = scoreBoard.FinishMatch(addResult.Data.Value);

            // assert
            Assert.True(finishMatchResult.IsSuccess);
            Assert.True(finishMatchResult.Data);
            Assert.Null(finishMatchResult.Errors);
        }

        [Fact]
        public void FinishMatch_NotExistingMatch_ReturnsFalse()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            // act
            OperationResult<bool> finishMatchResult = scoreBoard.FinishMatch(Guid.NewGuid());

            // assert
            Assert.False(finishMatchResult.IsSuccess);
            Assert.False(finishMatchResult.Data);
            Assert.NotNull(finishMatchResult.Errors);
        }

        [Fact]
        public void FinishMatch_ConcurrentWrite_ReturnsResult()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            var matchesToAdd = new List<Tuple<string, string, DateTime>>();

            for (int i = 0; i < 1000; i++)
            {
                matchesToAdd.Add(new($"home{i}", $"away{i}", DateTime.Now));
            }

            ConcurrentBag<Guid> ids = [];
            ConcurrentBag<bool> results = [];
            // act
            Parallel.ForEach(matchesToAdd, match =>
            {
                var result = scoreBoard.AddMatch(match.Item1, match.Item2, match.Item3);
                ids.Add(result.Data.Value);
            });

            Parallel.ForEach(ids, id =>
            {
                var result = scoreBoard.FinishMatch(id);
                results.Add(result.IsSuccess);
            });

            // assert
            Assert.Equal(results.Count, 1000);
            Assert.All(results, Assert.True);
        }

        [Fact]
        public void GetLiveMatches_SortedByScore_ReturnsSorted()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            // act
            OperationResult<Guid?> addResult = scoreBoard.AddMatch("team1", "team2", DateTime.Now);
            scoreBoard.UpdateScore(addResult.Data.Value, 2, 0);

            OperationResult<Guid?> addResult2 = scoreBoard.AddMatch("team3", "team4", DateTime.Now);
            scoreBoard.UpdateScore(addResult2.Data.Value, 3, 0);

            OperationResult<Guid?> addResult3 = scoreBoard.AddMatch("team5", "team6", DateTime.Now);
            scoreBoard.UpdateScore(addResult3.Data.Value, 2, 2);

            var expectedOrder = new Guid[3] { addResult3.Data.Value, addResult2.Data.Value, addResult.Data.Value };

            OperationResult<MatchDto[]> getMatchesResult = scoreBoard.GetLiveMatches();

            // assert
            Assert.True(getMatchesResult.IsSuccess);
            Assert.NotNull(getMatchesResult.Data);
            Assert.NotEmpty(getMatchesResult.Data);
            Assert.Null(getMatchesResult.Errors);
            Assert.Equal(expectedOrder, getMatchesResult.Data.Select(m => m.Id));
        }

        [Fact]
        public void GetLiveMatches_SortedByScoreThenByDate_ReturnsSorted()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            // act
            OperationResult<Guid?> addResult = scoreBoard.AddMatch("team1", "team2", DateTime.Now);
            scoreBoard.UpdateScore(addResult.Data.Value, 2, 0);

            OperationResult<Guid?> addResult2 = scoreBoard.AddMatch("team3", "team4", DateTime.Now - TimeSpan.FromMinutes(1));
            scoreBoard.UpdateScore(addResult2.Data.Value, 1, 1);

            OperationResult<Guid?> addResult3 = scoreBoard.AddMatch("team5", "team6", DateTime.Now - TimeSpan.FromMinutes(2));
            scoreBoard.UpdateScore(addResult3.Data.Value, 0, 2);

            var expectedOrder = new Guid[3] { addResult3.Data.Value, addResult2.Data.Value, addResult.Data.Value };

            OperationResult<MatchDto[]> getMatchesResult = scoreBoard.GetLiveMatches();

            // assert
            Assert.True(getMatchesResult.IsSuccess);
            Assert.NotNull(getMatchesResult.Data);
            Assert.NotEmpty(getMatchesResult.Data);
            Assert.Null(getMatchesResult.Errors);
            Assert.Equal(expectedOrder, getMatchesResult.Data.Select(m => m.Id));
        }

        [Fact]
        public void GetLiveMatches_ConcurrentAccess_ReturnsResult()
        {
            // arrange
            var scoreBoard = new LiveScoreboard(new MatchStorage());

            var matchesToAdd = new List<Tuple<string, string, DateTime>>();

            for (int i = 0; i < 1000; i++)
            {
                matchesToAdd.Add(new($"home{i}", $"away{i}", DateTime.Now));
            }

            ConcurrentBag<Guid> ids = [];
            ConcurrentBag<bool> results = [];
            // act
            Parallel.ForEach(matchesToAdd, match =>
            {
                var result = scoreBoard.AddMatch(match.Item1, match.Item2, match.Item3);
                ids.Add(result.Data.Value);
            });

            Parallel.ForEach(ids, id =>
            {
                var result = scoreBoard.GetLiveMatches();
                results.Add(result.IsSuccess);
                Assert.Equal(1000, result.Data.Length);
            });

            // assert
            Assert.Equal(1000, results.Count);
            Assert.All(results, Assert.True);
        }
    }
}

