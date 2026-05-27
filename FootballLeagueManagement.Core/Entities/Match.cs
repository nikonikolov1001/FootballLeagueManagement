using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Entities;

public class Match
{
    public int Id { get; set; }

    public int HomeTeamId { get; set; }
    public Team? HomeTeam { get; set; }

    public int AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }

    public int StadiumId { get; set; }
    public Stadium? Stadium { get; set; }

    public DateTime MatchDate { get; set; }

    [MaxLength(80)]
    public string Competition { get; set; } = "League";

    [Range(0, 30)]
    public int? HomeGoals { get; set; }

    [Range(0, 30)]
    public int? AwayGoals { get; set; }

    public bool IsFinished => HomeGoals.HasValue && AwayGoals.HasValue;
}
