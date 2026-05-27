using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Entities;

public class Stadium
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string City { get; set; } = string.Empty;

    [Range(1000, 150000)]
    public int Capacity { get; set; }

    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
