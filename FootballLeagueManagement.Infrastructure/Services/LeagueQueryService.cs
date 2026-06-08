using FootballLeagueManagement.Core.Dtos;
using FootballLeagueManagement.Core.Services;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Infrastructure.Services;

public class LeagueQueryService(ApplicationDbContext dbContext) : ILeagueQueryService
{
    public async Task<IReadOnlyList<StandingDto>> GetStandingsAsync(CancellationToken cancellationToken = default)
    {
        var clubs = await dbContext.Clubs
            .AsNoTracking()
            .OrderBy(club => club.Name)
            .ToListAsync(cancellationToken);

        var matches = await dbContext.Matches
            .AsNoTracking()
            .Where(match => match.HomeGoals.HasValue && match.AwayGoals.HasValue)
            .ToListAsync(cancellationToken);

        var rows = clubs.ToDictionary(club => club.Id, club => new StandingAccumulator(club.Name));

        foreach (var match in matches)
        {
            rows[match.HomeClubId].Apply(match.HomeGoals!.Value, match.AwayGoals!.Value);
            rows[match.AwayClubId].Apply(match.AwayGoals!.Value, match.HomeGoals!.Value);
        }

        return rows.Values
            .Select(row => row.ToDto())
            .OrderByDescending(row => row.Points)
            .ThenByDescending(row => row.GoalDifference)
            .ThenByDescending(row => row.GoalsFor)
            .ThenBy(row => row.Club)
            .Select((row, index) => row with { Position = index + 1 })
            .ToList();
    }

    public async Task<LeagueSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var standings = await GetStandingsAsync(cancellationToken);
        var clubs = await dbContext.Clubs.CountAsync(cancellationToken);
        var players = await dbContext.Players.CountAsync(cancellationToken);
        var stadiums = await dbContext.Stadiums.CountAsync(cancellationToken);
        var matches = await dbContext.Matches.CountAsync(cancellationToken);
        var playedMatches = await dbContext.Matches.CountAsync(match => match.HomeGoals.HasValue && match.AwayGoals.HasValue, cancellationToken);

        return new LeagueSummaryDto(clubs, players, stadiums, matches, playedMatches, standings.FirstOrDefault()?.Club ?? "No leader");
    }

    private sealed class StandingAccumulator(string club)
    {
        private int _played;
        private int _wins;
        private int _draws;
        private int _losses;
        private int _goalsFor;
        private int _goalsAgainst;

        public void Apply(int goalsFor, int goalsAgainst)
        {
            _played++;
            _goalsFor += goalsFor;
            _goalsAgainst += goalsAgainst;

            if (goalsFor > goalsAgainst)
            {
                _wins++;
            }
            else if (goalsFor == goalsAgainst)
            {
                _draws++;
            }
            else
            {
                _losses++;
            }
        }

        public StandingDto ToDto() =>
            new(0, club, _played, _wins, _draws, _losses, _goalsFor, _goalsAgainst, _goalsFor - _goalsAgainst, _wins * 3 + _draws);
    }
}
