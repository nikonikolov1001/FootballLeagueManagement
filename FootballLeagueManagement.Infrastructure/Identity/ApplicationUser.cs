using Microsoft.AspNetCore.Identity;

namespace FootballLeagueManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string FavoriteClub { get; set; } = string.Empty;
}
