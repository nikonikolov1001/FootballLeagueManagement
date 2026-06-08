using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Models.Api;

public class PlayerInputModel
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Position { get; set; } = string.Empty;

    [Range(1, 99)]
    public int ShirtNumber { get; set; }

    [Range(1, int.MaxValue)]
    public int ClubId { get; set; }
}
