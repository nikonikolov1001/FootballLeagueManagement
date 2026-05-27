using FootballLeagueManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FootballLeagueManagement.Infrastructure.Data;

public static class DatabaseSeeder
{
    public const string AdministratorRole = "Administrator";

    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AdministratorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdministratorRole));
        }

        const string adminEmail = "admin@football.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Project Administrator",
                FavoriteTeam = "Sofia Lions"
            };

            await userManager.CreateAsync(admin, "Admin123");
        }

        if (!await userManager.IsInRoleAsync(admin, AdministratorRole))
        {
            await userManager.AddToRoleAsync(admin, AdministratorRole);
        }
    }
}
