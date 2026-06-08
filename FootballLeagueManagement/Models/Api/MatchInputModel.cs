using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Models.Api;

public class MatchInputModel
{
    [Range(1, int.MaxValue)]
    public int HomeClubId { get; set; }

    [Range(1, int.MaxValue)]
    public int AwayClubId { get; set; }

    public DateTime KickoffUtc { get; set; }
}
