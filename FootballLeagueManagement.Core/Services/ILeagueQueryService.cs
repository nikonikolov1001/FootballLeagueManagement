using FootballLeagueManagement.Core.Dtos;

namespace FootballLeagueManagement.Core.Services;

public interface ILeagueQueryService
{
    Task<IReadOnlyList<StandingDto>> GetStandingsAsync(CancellationToken cancellationToken = default);

    Task<LeagueSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
