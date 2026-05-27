using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public record PlayerDto(int Id, string FirstName, string LastName, string Position, int ShirtNumber, string Nationality, int TeamId, string? TeamName);

public class PlayerRequest
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Position { get; set; } = string.Empty;

    [Range(1, 99)]
    public int ShirtNumber { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nationality { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TeamId { get; set; }
}
