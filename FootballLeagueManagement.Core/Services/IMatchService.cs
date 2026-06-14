using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Core.Services;

public interface IMatchService
{
    Task<Match> CreateAsync(int homeClubId, int awayClubId, DateTime kickoffUtc, CancellationToken cancellationToken = default);

    Task<Match> UpdateAsync(int matchId, int homeClubId, int awayClubId, DateTime kickoffUtc, CancellationToken cancellationToken = default);

    Task<Match> RecordResultAsync(int matchId, int homeGoals, int awayGoals, CancellationToken cancellationToken = default);

    Task<Match> DeleteAsync(int matchId, CancellationToken cancellationToken = default);
}
