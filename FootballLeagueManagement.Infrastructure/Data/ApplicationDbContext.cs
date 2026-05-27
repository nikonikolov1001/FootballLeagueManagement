using FootballLeagueManagement.Core.Entities;
using FootballLeagueManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Stadium> Stadiums => Set<Stadium>();
    public DbSet<Match> Matches => Set<Match>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Team>()
            .HasOne(t => t.Stadium)
            .WithMany(s => s.Teams)
            .HasForeignKey(t => t.StadiumId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Player>()
            .HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.Stadium)
            .WithMany(s => s.Matches)
            .HasForeignKey(m => m.StadiumId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Stadium>().HasData(
            new Stadium { Id = 1, Name = "National Stadium", City = "Sofia", Capacity = 43000 },
            new Stadium { Id = 2, Name = "Green Arena", City = "Plovdiv", Capacity = 22000 });

        builder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Sofia Lions", City = "Sofia", FoundedYear = 1948, CoachName = "Ivan Petrov", StadiumId = 1 },
            new Team { Id = 2, Name = "Plovdiv Eagles", City = "Plovdiv", FoundedYear = 1936, CoachName = "Georgi Ivanov", StadiumId = 2 });

        builder.Entity<Player>().HasData(
            new Player { Id = 1, FirstName = "Nikolay", LastName = "Dimitrov", Position = "Forward", ShirtNumber = 9, Nationality = "Bulgaria", TeamId = 1 },
            new Player { Id = 2, FirstName = "Martin", LastName = "Kolev", Position = "Midfielder", ShirtNumber = 8, Nationality = "Bulgaria", TeamId = 1 },
            new Player { Id = 3, FirstName = "Petar", LastName = "Stoyanov", Position = "Goalkeeper", ShirtNumber = 1, Nationality = "Bulgaria", TeamId = 2 });

        builder.Entity<Match>().HasData(
            new Match { Id = 1, HomeTeamId = 1, AwayTeamId = 2, StadiumId = 1, MatchDate = new DateTime(2026, 6, 15, 19, 30, 0), Competition = "First League", HomeGoals = 2, AwayGoals = 1 },
            new Match { Id = 2, HomeTeamId = 2, AwayTeamId = 1, StadiumId = 2, MatchDate = new DateTime(2026, 7, 5, 20, 0, 0), Competition = "First League" });
    }
}
