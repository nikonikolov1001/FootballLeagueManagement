using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Club> Clubs => Set<Club>();

    public DbSet<Player> Players => Set<Player>();

    public DbSet<Stadium> Stadiums => Set<Stadium>();

    public DbSet<Match> Matches => Set<Match>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Club>()
            .HasIndex(club => club.Name)
            .IsUnique();

        builder.Entity<Club>()
            .HasOne(club => club.Stadium)
            .WithOne(stadium => stadium.Club)
            .HasForeignKey<Club>(club => club.StadiumId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Player>()
            .HasOne(player => player.Club)
            .WithMany(club => club.Players)
            .HasForeignKey(player => player.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Match>()
            .HasOne(match => match.HomeClub)
            .WithMany()
            .HasForeignKey(match => match.HomeClubId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(match => match.AwayClub)
            .WithMany()
            .HasForeignKey(match => match.AwayClubId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedRoles(builder);
        SeedUsers(builder);
        SeedLeague(builder);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "role-admin",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "role-user",
                Name = "User",
                NormalizedName = "USER"
            });
    }

    private static void SeedUsers(ModelBuilder builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        var admin = new ApplicationUser
        {
            Id = "user-admin",
            UserName = "admin@premierleague.local",
            NormalizedUserName = "ADMIN@PREMIERLEAGUE.LOCAL",
            Email = "admin@premierleague.local",
            NormalizedEmail = "ADMIN@PREMIERLEAGUE.LOCAL",
            EmailConfirmed = true,
            FullName = "Premier League Administrator",
            FavoriteClub = "Liverpool"
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        builder.Entity<ApplicationUser>().HasData(admin);
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            UserId = admin.Id,
            RoleId = "role-admin"
        });
    }

    private static void SeedLeague(ModelBuilder builder)
    {
        builder.Entity<Stadium>().HasData(
            new Stadium { Id = 1, Name = "Anfield", City = "Liverpool", Capacity = 61276 },
            new Stadium { Id = 2, Name = "Emirates Stadium", City = "London", Capacity = 60383 },
            new Stadium { Id = 3, Name = "Etihad Stadium", City = "Manchester", Capacity = 53400 },
            new Stadium { Id = 4, Name = "Old Trafford", City = "Manchester", Capacity = 74310 },
            new Stadium { Id = 5, Name = "Stamford Bridge", City = "London", Capacity = 41837 },
            new Stadium { Id = 6, Name = "Tottenham Hotspur Stadium", City = "London", Capacity = 62850 },
            new Stadium { Id = 7, Name = "St James' Park", City = "Newcastle upon Tyne", Capacity = 52305 },
            new Stadium { Id = 8, Name = "Villa Park", City = "Birmingham", Capacity = 42657 });

        builder.Entity<Club>().HasData(
            new Club { Id = 1, Name = "Liverpool", ShortCode = "LIV", City = "Liverpool", FoundedYear = 1892, StadiumId = 1 },
            new Club { Id = 2, Name = "Arsenal", ShortCode = "ARS", City = "London", FoundedYear = 1886, StadiumId = 2 },
            new Club { Id = 3, Name = "Manchester City", ShortCode = "MCI", City = "Manchester", FoundedYear = 1880, StadiumId = 3 },
            new Club { Id = 4, Name = "Manchester United", ShortCode = "MUN", City = "Manchester", FoundedYear = 1878, StadiumId = 4 },
            new Club { Id = 5, Name = "Chelsea", ShortCode = "CHE", City = "London", FoundedYear = 1905, StadiumId = 5 },
            new Club { Id = 6, Name = "Tottenham Hotspur", ShortCode = "TOT", City = "London", FoundedYear = 1882, StadiumId = 6 },
            new Club { Id = 7, Name = "Newcastle United", ShortCode = "NEW", City = "Newcastle upon Tyne", FoundedYear = 1892, StadiumId = 7 },
            new Club { Id = 8, Name = "Aston Villa", ShortCode = "AVL", City = "Birmingham", FoundedYear = 1874, StadiumId = 8 });

        builder.Entity<Player>().HasData(
            new Player { Id = 1, FullName = "Mohamed Salah", Position = "Forward", ShirtNumber = 11, ClubId = 1 },
            new Player { Id = 2, FullName = "Bukayo Saka", Position = "Forward", ShirtNumber = 7, ClubId = 2 },
            new Player { Id = 3, FullName = "Erling Haaland", Position = "Forward", ShirtNumber = 9, ClubId = 3 },
            new Player { Id = 4, FullName = "Bruno Fernandes", Position = "Midfielder", ShirtNumber = 8, ClubId = 4 },
            new Player { Id = 5, FullName = "Cole Palmer", Position = "Midfielder", ShirtNumber = 20, ClubId = 5 },
            new Player { Id = 6, FullName = "Son Heung-min", Position = "Forward", ShirtNumber = 7, ClubId = 6 });

        builder.Entity<Match>().HasData(
            new Match { Id = 1, HomeClubId = 1, AwayClubId = 2, KickoffUtc = new DateTime(2025, 9, 20, 15, 0, 0, DateTimeKind.Utc), HomeGoals = 2, AwayGoals = 1 },
            new Match { Id = 2, HomeClubId = 3, AwayClubId = 5, KickoffUtc = new DateTime(2025, 9, 21, 16, 30, 0, DateTimeKind.Utc), HomeGoals = 3, AwayGoals = 1 },
            new Match { Id = 3, HomeClubId = 6, AwayClubId = 4, KickoffUtc = new DateTime(2025, 9, 22, 19, 0, 0, DateTimeKind.Utc), HomeGoals = 1, AwayGoals = 1 },
            new Match { Id = 4, HomeClubId = 7, AwayClubId = 8, KickoffUtc = new DateTime(2025, 10, 4, 14, 0, 0, DateTimeKind.Utc) });
    }
}
