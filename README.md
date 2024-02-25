# LiveScores
> Sports live scoreboard

## Table of Contents
* [General Info](#general-information)
* [Technologies Used](#technologies-used)
* [Features](#features)
* [Usage](#usage)
* [Project Status](#project-status)
* [Room for Improvement](#room-for-improvement)
* [Contact](#contact)

## General Information
- Live scoreboard with ability to add, update and finish matches

## Technologies Used
- .NET 8
- xUnit 2.7.0

## Features
- Add Match
- Finish Match
- Update Score
- Get Live Matches

## Usage
```
public interface ILiveScoreboard
{
    OperationResult<Guid?> AddMatch(string homeTeam, string awayTeam, DateTime started);

    OperationResult<bool> UpdateScore(Guid matchId, byte newHomeTeamScore, byte newAwayTeamScore);

    OperationResult<bool> FinishMatch(Guid matchId);

    OperationResult<MatchDto[]> GetLiveMatches();
}
public record OperationResult<T>(T? Data, bool IsSuccess, IDictionary<string, string>? Errors);
```
## Project Status
Project is:_complete_

## Room for Improvement
- Persistence for finished matches
- Admin UI
- LiveScores UI 

## Contact
Created by @gerakirill - feel free to contact me!
