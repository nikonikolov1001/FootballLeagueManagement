using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagement.Models.Api;

public class LoginInputModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
