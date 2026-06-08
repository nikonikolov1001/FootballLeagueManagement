using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FootballLeagueManagement.Tests;

[TestFixture]
public class MatchServiceTests
{
    [Test]
    public async Task RecordResultAsync_UpdatesExistingMatch()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Stadiums.AddRange(
            new Stadium { Id = 1, Name = "Anfield", City = "Liverpool", Capacity = 61276 },
            new Stadium { Id = 2, Name = "Emirates Stadium", City = "London", Capacity = 60383 });
        dbContext.Clubs.AddRange(
            new Club { Id = 1, Name = "Liverpool", ShortCode = "LIV", City = "Liverpool", FoundedYear = 1892, StadiumId = 1 },
            new Club { Id = 2, Name = "Arsenal", ShortCode = "ARS", City = "London", FoundedYear = 1886, StadiumId = 2 });
        dbContext.Matches.Add(new Match
        {
            Id = 1,
            HomeClubId = 1,
            AwayClubId = 2,
            KickoffUtc = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var service = new MatchService(dbContext);
        var result = await service.RecordResultAsync(1, 2, 1);

        Assert.That(result.HomeGoals, Is.EqualTo(2));
        Assert.That(result.AwayGoals, Is.EqualTo(1));
    }

    [Test]
    public void RecordResultAsync_ThrowsForNegativeGoals()
    {
        using var dbContext = CreateDbContext();
        var service = new MatchService(dbContext);

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.RecordResultAsync(1, -1, 0));
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
