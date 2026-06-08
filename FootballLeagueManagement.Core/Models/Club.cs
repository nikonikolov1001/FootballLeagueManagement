using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Core.Models;

public class Club : Entity
{
    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(8)]
    public string ShortCode { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string City { get; set; } = string.Empty;

    public int FoundedYear { get; set; }

    public int StadiumId { get; set; }

    public Stadium? Stadium { get; set; }

    public ICollection<Player> Players { get; set; } = new List<Player>();
}
