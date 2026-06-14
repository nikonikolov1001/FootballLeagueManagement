using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Models.Api;

namespace FootballLeagueManagement.Services;

public interface IStadiumAdminService
{
    Task<AdminOperationResult<Stadium>> CreateAsync(StadiumInputModel input, CancellationToken cancellationToken = default);

    Task<AdminOperationResult<Stadium>> UpdateAsync(int id, StadiumInputModel input, CancellationToken cancellationToken = default);

    Task<AdminOperationResult<Stadium>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
