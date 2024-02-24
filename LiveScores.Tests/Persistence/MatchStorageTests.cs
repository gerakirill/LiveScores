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
        OperationResult<bool> result = storage.Add(match);

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
        OperationResult<Match?> result = storage.Get(id);
            
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
        OperationResult<bool> updateResult = storage.Update(match);
        OperationResult<Match?> getResult = storage.Get(id);

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
        OperationResult<bool> deleteResult = storage.Delete(id);
        OperationResult<Match?> getResult = storage.Get(id);

        // assert
        Assert.True(deleteResult.Data);
        Assert.True(deleteResult.IsSuccess);
        Assert.False(getResult.IsSuccess);
        Assert.NotNull(getResult.Errors);
    }

    [Fact]
    public void Add_DuplicateKey_ReturnsFalse()
    {
        // arrange
        var storage = new MatchStorage();
        var match = new Match("team1", "team2", DateTime.Now);
        Guid id = match.Id;
        var match2 = new Match("team1", "team2", DateTime.Now);
        match2 = match2 with { Id = id };

        // act
        OperationResult<bool> addFirstResult = storage.Add(match);
        OperationResult<bool> addSecondResult = storage.Add(match2);

        // assert
        Assert.True(addFirstResult.IsSuccess);
        Assert.True(addFirstResult.Data);
        Assert.False(addSecondResult.IsSuccess);
        Assert.False(addSecondResult.Data);
        Assert.NotNull(addSecondResult.Errors);
    }

    [Fact]
    public void Get_NotExistingMatch_ReturnsNull()
    {
        //arrange
        var storage = new MatchStorage();
        Guid id = Guid.NewGuid();

        // act
        OperationResult<Match?> result = storage.Get(id);

        // assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Update_NotExistingMatch_ReturnsFalse()
    {
        // arrange
        var storage = new MatchStorage();
        var match = new Match("team1", "team2", DateTime.Now);
        Guid id = match.Id;
        byte updatedHomeTeamScore = 1;
        byte updatedAwayTeamScore = 0;
        match.UpdateScore(updatedHomeTeamScore, updatedAwayTeamScore);

        // act
        OperationResult<bool> updateResult = storage.Update(match);

        //assert
        Assert.False(updateResult.IsSuccess);
        Assert.False(updateResult.Data);
        Assert.NotNull(updateResult.Errors);
    }

    [Fact]
    public void Delete_NotExistingMatch_ReturnsFalse()
    {
        // arrange
        var storage = new MatchStorage();
        Guid id = Guid.NewGuid();

        // act
        OperationResult<bool> deleteResult = storage.Delete(id);

        // assert
        Assert.False(deleteResult.Data);
        Assert.False(deleteResult.IsSuccess);
        Assert.NotNull(deleteResult.Errors);
    }
}