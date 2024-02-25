namespace LiveScores.Application.Contracts;

public record MatchDto(Guid Id, string HomeTeam, string AwayTeam, byte HomeTeamScore, byte AwayTeamScore, DateTime Started);