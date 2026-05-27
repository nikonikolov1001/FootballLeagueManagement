using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public record StadiumDto(int Id, string Name, string City, int Capacity);

public class StadiumRequest
{
    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string City { get; set; } = string.Empty;

    [Range(1000, 150000)]
    public int Capacity { get; set; }
}
