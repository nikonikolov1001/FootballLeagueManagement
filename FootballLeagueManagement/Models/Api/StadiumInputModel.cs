using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Models.Api;

public class StadiumInputModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string City { get; set; } = string.Empty;

    [Range(1000, 100000)]
    public int Capacity { get; set; }
}
