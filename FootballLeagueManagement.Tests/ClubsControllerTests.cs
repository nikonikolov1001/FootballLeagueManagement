using FootballLeagueManagement.Controllers;
using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using FootballLeagueManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace FootballLeagueManagement.Tests;

[TestFixture]
public class ClubsControllerTests
{
    [Test]
    public async Task Create_ReturnsCreatedResult_WhenServiceCreatesClub()
    {
        await using var dbContext = CreateDbContext();
        var club = new Club { Id = 10, Name = "Test Club", ShortCode = "TST", City = "London", FoundedYear = 1900, StadiumId = 1 };
        var service = new Mock<IClubAdminService>();
        service
            .Setup(service => service.CreateAsync(It.IsAny<ClubInputModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AdminOperationResult<Club>.Success(club));

        var controller = new ClubsController(dbContext, CreateConfiguration(), service.Object);

        var result = await controller.Create(new ClubInputModel
        {
            Name = "Test Club",
            ShortCode = "TST",
            City = "London",
            FoundedYear = 1900,
            StadiumId = 1
        }, CancellationToken.None);

        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task Create_ReturnsConflict_WhenServiceFindsDuplicateClub()
    {
        await using var dbContext = CreateDbContext();
        var service = new Mock<IClubAdminService>();
        service
            .Setup(service => service.CreateAsync(It.IsAny<ClubInputModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AdminOperationResult<Club>.Conflict("A club with the same name or short code already exists."));

        var controller = new ClubsController(dbContext, CreateConfiguration(), service.Object);

        var result = await controller.Create(new ClubInputModel
        {
            Name = "Test Club",
            ShortCode = "TST",
            City = "London",
            FoundedYear = 1900,
            StadiumId = 1
        }, CancellationToken.None);

        Assert.That(result, Is.TypeOf<ConflictObjectResult>());
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseDemoData"] = "false"
            })
            .Build();
    }
}
