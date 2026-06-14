using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Models.Api;

namespace FootballLeagueManagement.Services;

public interface IPlayerAdminService
{
    Task<AdminOperationResult<Player>> CreateAsync(PlayerInputModel input, CancellationToken cancellationToken = default);

    Task<AdminOperationResult<Player>> UpdateAsync(int id, PlayerInputModel input, CancellationToken cancellationToken = default);

    Task<AdminOperationResult<Player>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
