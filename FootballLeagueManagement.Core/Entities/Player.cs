using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Entities;

public class Player
{
    public int Id { get; set; }

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

    public int TeamId { get; set; }
    public Team? Team { get; set; }
}
