using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Models.Api;

public class ClubInputModel
{
    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(8)]
    public string ShortCode { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string City { get; set; } = string.Empty;

    [Range(1800, 2100)]
    public int FoundedYear { get; set; }

    [Range(1, int.MaxValue)]
    public int StadiumId { get; set; }
}
