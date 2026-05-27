using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Entities;
using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Core.Services;

public class TeamService : ITeamService
{
    private readonly IRepository<Team> teams;

    public TeamService(IRepository<Team> teams)
    {
        this.teams = teams;
    }

    public Task<IReadOnlyCollection<TeamDto>> GetAllAsync(string? search = null)
    {
        var query = teams.All();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Name.Contains(search) || t.City.Contains(search));
        }

        var result = query
            .OrderBy(t => t.Name)
            .AsEnumerable()
            .Select(ToDto)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<TeamDto>>(result);
    }

    public async Task<TeamDto?> GetByIdAsync(int id)
    {
        var team = await teams.FindAsync(id);
        return team is null ? null : ToDto(team);
    }

    public async Task<TeamDto> CreateAsync(TeamRequest request)
    {
        var team = new Team
        {
            Name = request.Name,
            City = request.City,
            FoundedYear = request.FoundedYear,
            CoachName = request.CoachName,
            StadiumId = request.StadiumId
        };

        await teams.AddAsync(team);
        await teams.SaveChangesAsync();

        return ToDto(team);
    }

    public async Task<bool> UpdateAsync(int id, TeamRequest request)
    {
        var team = await teams.FindAsync(id);
        if (team is null)
        {
            return false;
        }

        team.Name = request.Name;
        team.City = request.City;
        team.FoundedYear = request.FoundedYear;
        team.CoachName = request.CoachName;
        team.StadiumId = request.StadiumId;
        await teams.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var team = await teams.FindAsync(id);
        if (team is null)
        {
            return false;
        }

        teams.Remove(team);
        await teams.SaveChangesAsync();

        return true;
    }

    private static TeamDto ToDto(Team team)
        => new(team.Id, team.Name, team.City, team.FoundedYear, team.CoachName, team.Stadium?.Name);
}
