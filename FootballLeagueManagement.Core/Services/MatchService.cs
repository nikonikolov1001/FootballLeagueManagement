using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Entities;
using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Core.Services;

public class MatchService : IMatchService
{
    private readonly IRepository<Match> matches;

    public MatchService(IRepository<Match> matches)
    {
        this.matches = matches;
    }

    public Task<IReadOnlyCollection<MatchDto>> GetAllAsync(bool? finished = null)
    {
        var query = matches.All();

        if (finished.HasValue)
        {
            query = query.Where(m => m.IsFinished == finished.Value);
        }

        var result = query.OrderBy(m => m.MatchDate).AsEnumerable().Select(ToDto).ToList().AsReadOnly();
        return Task.FromResult<IReadOnlyCollection<MatchDto>>(result);
    }

    public async Task<MatchDto?> GetByIdAsync(int id)
    {
        var match = await matches.FindAsync(id);
        return match is null ? null : ToDto(match);
    }

    public async Task<MatchDto> CreateAsync(MatchRequest request)
    {
        var match = new Match
        {
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            StadiumId = request.StadiumId,
            MatchDate = request.MatchDate,
            Competition = request.Competition
        };

        await matches.AddAsync(match);
        await matches.SaveChangesAsync();
        return ToDto(match);
    }

    public async Task<bool> UpdateAsync(int id, MatchRequest request)
    {
        var match = await matches.FindAsync(id);
        if (match is null)
        {
            return false;
        }

        match.HomeTeamId = request.HomeTeamId;
        match.AwayTeamId = request.AwayTeamId;
        match.StadiumId = request.StadiumId;
        match.MatchDate = request.MatchDate;
        match.Competition = request.Competition;
        await matches.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetResultAsync(int id, MatchResultRequest request)
    {
        var match = await matches.FindAsync(id);
        if (match is null)
        {
            return false;
        }

        match.HomeGoals = request.HomeGoals;
        match.AwayGoals = request.AwayGoals;
        await matches.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var match = await matches.FindAsync(id);
        if (match is null)
        {
            return false;
        }

        matches.Remove(match);
        await matches.SaveChangesAsync();
        return true;
    }

    private static MatchDto ToDto(Match match)
        => new(
            match.Id,
            match.HomeTeam?.Name ?? $"Team #{match.HomeTeamId}",
            match.AwayTeam?.Name ?? $"Team #{match.AwayTeamId}",
            match.Stadium?.Name ?? $"Stadium #{match.StadiumId}",
            match.MatchDate,
            match.Competition,
            match.HomeGoals,
            match.AwayGoals);
}
