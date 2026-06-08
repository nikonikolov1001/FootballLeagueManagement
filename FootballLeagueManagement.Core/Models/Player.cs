using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public class Player : Entity
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Position { get; set; } = string.Empty;

    [Range(1, 99)]
    public int ShirtNumber { get; set; }

    public int ClubId { get; set; }

    public Club? Club { get; set; }
}
