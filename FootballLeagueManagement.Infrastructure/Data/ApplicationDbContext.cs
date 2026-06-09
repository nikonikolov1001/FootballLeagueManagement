using FootballLeagueManagement.Core.Data;
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
        builder.Entity<Stadium>().HasData(PremierLeagueSnapshot.Stadiums.Select(stadium => new Stadium
        {
            Id = stadium.Id,
            Name = stadium.Name,
            City = stadium.City,
            Capacity = stadium.Capacity
        }));

        builder.Entity<Club>().HasData(PremierLeagueSnapshot.Clubs.Select(club => new Club
        {
            Id = club.Id,
            Name = club.Name,
            ShortCode = club.ShortCode,
            City = club.City,
            FoundedYear = club.FoundedYear,
            StadiumId = club.StadiumId
        }));

        builder.Entity<Player>().HasData(PremierLeagueSnapshot.Players.Select(player => new Player
        {
            Id = player.Id,
            FullName = player.FullName,
            Position = player.Position,
            ShirtNumber = player.ShirtNumber,
            ClubId = player.ClubId
        }));

        builder.Entity<Match>().HasData(PremierLeagueSnapshot.Matches.Select(match => new Match
        {
            Id = match.Id,
            HomeClubId = match.HomeClubId,
            AwayClubId = match.AwayClubId,
            KickoffUtc = match.KickoffUtc,
            HomeGoals = match.HomeGoals,
            AwayGoals = match.AwayGoals
        }));
    }
}
