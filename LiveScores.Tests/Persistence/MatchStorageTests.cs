using LiveScores.Application.Contracts;
using LiveScores.Domain.Entities;
using LiveScores.Persistence;
using Xunit;

namespace LiveScores.Tests.Persistence;

public class MatchStorageTests
{
    [Fact]
    public void Add_ValidMatch_ReturnsTrue()
    {
        // arrange
        var storage = new MatchStorage();
        var match = new Match("team1", "team2", DateTime.Now);

        // act
        PersistedResult<bool> result = storage.Add(match);

        // assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    [Fact]
    public void Get_ExistingMatch_ReturnsMatch()
    {
        //arrange
        var storage = new MatchStorage();
        var match = new Match("team1", "team2", DateTime.Now);
        Guid id = match.Id;
        storage.Add(match);

        // act
        PersistedResult<Match?> result = storage.Get(id);
            
        // assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Errors);
        Assert.Equal(result.Data, match);
    }

    [Fact]
    public void Update_ExistingMatch_ReturnsTrue()
    {
        // arrange
        var storage = new MatchStorage();
        var match = new Match("team1", "team2", DateTime.Now);
        Guid id = match.Id;
        storage.Add(match);
        byte updatedHomeTeamScore = 1;
        byte updatedAwayTeamScore = 0;
        match.UpdateScore(updatedHomeTeamScore, updatedAwayTeamScore);

        // act
        PersistedResult<bool> updateResult = storage.Update(match);
        PersistedResult<Match?> getResult = storage.Get(id);

        //assert
        Assert.True(updateResult.IsSuccess);
        Assert.True(updateResult.Data);
        Assert.True(getResult.IsSuccess);
        Assert.Null(getResult.Errors);
        Assert.Equal(getResult.Data, match);
    }

    [Fact]
    public void Delete_ExistingMatch_ReturnsTrue()
    {
        // arrange
        var storage = new MatchStorage();
        var match = new Match("team1", "team2", DateTime.Now);
        Guid id = match.Id;
        storage.Add(match);

        // act
        PersistedResult<bool> deleteResult = storage.Delete(id);
        PersistedResult<Match?> getResult = storage.Get(id);

        // assert
        Assert.True(deleteResult.Data);
        Assert.True(deleteResult.IsSuccess);
        Assert.False(getResult.IsSuccess);
        Assert.NotNull(getResult.Errors);
    }
}