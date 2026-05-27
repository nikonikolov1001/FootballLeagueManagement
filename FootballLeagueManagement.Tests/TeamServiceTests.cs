using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Entities;
using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Core.Services;
using Moq;

namespace FootballLeagueManagement.Tests;

public class TeamServiceTests
{
    [Test]
    public async Task GetAllAsync_FiltersTeamsBySearchText()
    {
        var repository = new Mock<IRepository<Team>>();
        repository
            .Setup(r => r.All())
            .Returns(new List<Team>
            {
                new() { Id = 1, Name = "Sofia Lions", City = "Sofia", FoundedYear = 1948, CoachName = "Ivan Petrov" },
                new() { Id = 2, Name = "Plovdiv Eagles", City = "Plovdiv", FoundedYear = 1936, CoachName = "Georgi Ivanov" }
            }.AsQueryable());

        var service = new TeamService(repository.Object);

        var result = await service.GetAllAsync("Sofia");

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Sofia Lions"));
    }

    [Test]
    public async Task CreateAsync_AddsTeamAndSavesChanges()
    {
        var repository = new Mock<IRepository<Team>>();
        var service = new TeamService(repository.Object);

        var request = new TeamRequest
        {
            Name = "Varna Sharks",
            City = "Varna",
            FoundedYear = 1962,
            CoachName = "Dimitar Todorov",
            StadiumId = 1
        };

        var result = await service.CreateAsync(request);

        Assert.That(result.Name, Is.EqualTo("Varna Sharks"));
        repository.Verify(r => r.AddAsync(It.Is<Team>(t => t.Name == "Varna Sharks")), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
