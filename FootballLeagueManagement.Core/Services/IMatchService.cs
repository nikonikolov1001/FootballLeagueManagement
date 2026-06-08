using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Core.Services;

public interface IMatchService
{
    Task<Match> RecordResultAsync(int matchId, int homeGoals, int awayGoals, CancellationToken cancellationToken = default);
}
