using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Entities;

public class Team
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string City { get; set; } = string.Empty;

    [Range(1850, 2100)]
    public int FoundedYear { get; set; }

    [Required]
    [MaxLength(80)]
    public string CoachName { get; set; } = string.Empty;

    public int? StadiumId { get; set; }
    public Stadium? Stadium { get; set; }

    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
    public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
}
