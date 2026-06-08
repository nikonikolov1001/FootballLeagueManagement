namespace FootballLeagueManagement.Core.Dtos;

public sealed record LeagueSummaryDto(int Clubs, int Players, int Stadiums, int Matches, int PlayedMatches, string Leader);
