namespace FootballLeagueManagement.Core.Dtos;

public sealed record StandingDto(
    int Position,
    string Club,
    int Played,
    int Wins,
    int Draws,
    int Losses,
    int GoalsFor,
    int GoalsAgainst,
    int GoalDifference,
    int Points);
