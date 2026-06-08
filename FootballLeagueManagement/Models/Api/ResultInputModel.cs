using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Models.Api;

public class ResultInputModel
{
    [Range(0, 30)]
    public int HomeGoals { get; set; }

    [Range(0, 30)]
    public int AwayGoals { get; set; }
}
