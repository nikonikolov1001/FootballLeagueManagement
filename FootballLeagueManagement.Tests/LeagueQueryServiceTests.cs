using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FootballLeagueManagement.Tests;

[TestFixture]
public class LeagueQueryServiceTests
{
    [Test]
    public async Task GetStandingsAsync_OrdersTeamsByPointsAndGoalDifference()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Stadiums.AddRange(
            new Stadium { Id = 1, Name = "Anfield", City = "Liverpool", Capacity = 61276 },
            new Stadium { Id = 2, Name = "Emirates Stadium", City = "London", Capacity = 60383 },
            new Stadium { Id = 3, Name = "Etihad Stadium", City = "Manchester", Capacity = 53400 });
        dbContext.Clubs.AddRange(
            new Club { Id = 1, Name = "Liverpool", ShortCode = "LIV", City = "Liverpool", FoundedYear = 1892, StadiumId = 1 },
            new Club { Id = 2, Name = "Arsenal", ShortCode = "ARS", City = "London", FoundedYear = 1886, StadiumId = 2 },
            new Club { Id = 3, Name = "Manchester City", ShortCode = "MCI", City = "Manchester", FoundedYear = 1880, StadiumId = 3 });
        dbContext.Matches.AddRange(
            new Match { Id = 1, HomeClubId = 1, AwayClubId = 2, KickoffUtc = DateTime.UtcNow, HomeGoals = 3, AwayGoals = 0 },
            new Match { Id = 2, HomeClubId = 3, AwayClubId = 2, KickoffUtc = DateTime.UtcNow, HomeGoals = 1, AwayGoals = 1 });
        await dbContext.SaveChangesAsync();

        var service = new LeagueQueryService(dbContext);
        var standings = await service.GetStandingsAsync();

        Assert.That(standings[0].Club, Is.EqualTo("Liverpool"));
        Assert.That(standings[0].Points, Is.EqualTo(3));
        Assert.That(standings[0].GoalDifference, Is.EqualTo(3));
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
