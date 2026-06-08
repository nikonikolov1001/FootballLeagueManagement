using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Models.Api;

public class RegisterInputModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string FavoriteClub { get; set; } = string.Empty;
}
