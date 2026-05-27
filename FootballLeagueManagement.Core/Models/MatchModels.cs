using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public record MatchDto(int Id, string HomeTeam, string AwayTeam, string Stadium, DateTime MatchDate, string Competition, int? HomeGoals, int? AwayGoals);

public class MatchRequest
{
    [Range(1, int.MaxValue)]
    public int HomeTeamId { get; set; }

    [Range(1, int.MaxValue)]
    public int AwayTeamId { get; set; }

    [Range(1, int.MaxValue)]
    public int StadiumId { get; set; }

    public DateTime MatchDate { get; set; }

    [Required]
    [MaxLength(80)]
    public string Competition { get; set; } = "League";
}

public class MatchResultRequest
{
    [Range(0, 30)]
    public int HomeGoals { get; set; }

    [Range(0, 30)]
    public int AwayGoals { get; set; }
}
