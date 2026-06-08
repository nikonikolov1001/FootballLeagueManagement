using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public class Match : Entity
{
    public int HomeClubId { get; set; }

    public Club? HomeClub { get; set; }

    public int AwayClubId { get; set; }

    public Club? AwayClub { get; set; }

    public DateTime KickoffUtc { get; set; }

    [Range(0, 30)]
    public int? HomeGoals { get; set; }

    [Range(0, 30)]
    public int? AwayGoals { get; set; }

    public bool IsPlayed => HomeGoals.HasValue && AwayGoals.HasValue;
}
