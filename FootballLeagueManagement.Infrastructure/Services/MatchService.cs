using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Core.Services;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Infrastructure.Services;

public class MatchService(ApplicationDbContext dbContext) : IMatchService
{
    public async Task<Match> CreateAsync(int homeClubId, int awayClubId, DateTime kickoffUtc, CancellationToken cancellationToken = default)
    {
        await ValidateClubsAsync(homeClubId, awayClubId, cancellationToken);

        var match = new Match
        {
            HomeClubId = homeClubId,
            AwayClubId = awayClubId,
            KickoffUtc = kickoffUtc
        };

        dbContext.Matches.Add(match);
        await dbContext.SaveChangesAsync(cancellationToken);
        return match;
    }

    public async Task<Match> UpdateAsync(int matchId, int homeClubId, int awayClubId, DateTime kickoffUtc, CancellationToken cancellationToken = default)
    {
        await ValidateClubsAsync(homeClubId, awayClubId, cancellationToken);

        var match = await FindMatchAsync(matchId, cancellationToken);
        match.HomeClubId = homeClubId;
        match.AwayClubId = awayClubId;
        match.KickoffUtc = kickoffUtc;
        await dbContext.SaveChangesAsync(cancellationToken);
        return match;
    }

    public async Task<Match> RecordResultAsync(int matchId, int homeGoals, int awayGoals, CancellationToken cancellationToken = default)
    {
        if (homeGoals < 0 || awayGoals < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(homeGoals), "Goals cannot be negative.");
        }

        var match = await FindMatchAsync(matchId, cancellationToken);
        match.HomeGoals = homeGoals;
        match.AwayGoals = awayGoals;
        await dbContext.SaveChangesAsync(cancellationToken);

        return match;
    }

    public async Task<Match> DeleteAsync(int matchId, CancellationToken cancellationToken = default)
    {
        var match = await FindMatchAsync(matchId, cancellationToken);
        dbContext.Matches.Remove(match);
        await dbContext.SaveChangesAsync(cancellationToken);
        return match;
    }

    private async Task<Match> FindMatchAsync(int matchId, CancellationToken cancellationToken)
    {
        return await dbContext.Matches.FirstOrDefaultAsync(match => match.Id == matchId, cancellationToken)
            ?? throw new InvalidOperationException("Match was not found.");
    }

    private async Task ValidateClubsAsync(int homeClubId, int awayClubId, CancellationToken cancellationToken)
    {
        if (homeClubId == awayClubId)
        {
            throw new ArgumentException("Home and away clubs must be different.");
        }

        var foundClubs = await dbContext.Clubs
            .CountAsync(club => club.Id == homeClubId || club.Id == awayClubId, cancellationToken);

        if (foundClubs != 2)
        {
            throw new ArgumentException("Both clubs must exist before creating a match.");
        }
    }
}
