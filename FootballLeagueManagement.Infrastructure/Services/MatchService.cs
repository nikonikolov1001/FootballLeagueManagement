using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Core.Services;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Infrastructure.Services;

public class MatchService(ApplicationDbContext dbContext) : IMatchService
{
    public async Task<Match> RecordResultAsync(int matchId, int homeGoals, int awayGoals, CancellationToken cancellationToken = default)
    {
        if (homeGoals < 0 || awayGoals < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(homeGoals), "Goals cannot be negative.");
        }

        var match = await dbContext.Matches.FirstOrDefaultAsync(match => match.Id == matchId, cancellationToken)
            ?? throw new InvalidOperationException("Match was not found.");

        match.HomeGoals = homeGoals;
        match.AwayGoals = awayGoals;
        await dbContext.SaveChangesAsync(cancellationToken);

        return match;
    }
}
