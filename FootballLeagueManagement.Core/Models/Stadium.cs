using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public class Stadium : Entity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string City { get; set; } = string.Empty;

    [Range(1000, 100000)]
    public int Capacity { get; set; }

    public Club? Club { get; set; }
}
