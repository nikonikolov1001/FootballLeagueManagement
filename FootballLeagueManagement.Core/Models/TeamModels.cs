using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public record TeamDto(int Id, string Name, string City, int FoundedYear, string CoachName, string? StadiumName);

public class TeamRequest
{
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
}
