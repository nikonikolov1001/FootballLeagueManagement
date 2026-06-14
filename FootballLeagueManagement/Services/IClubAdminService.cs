using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Models.Api;

namespace FootballLeagueManagement.Services;

public interface IClubAdminService
{
    Task<AdminOperationResult<Club>> CreateAsync(ClubInputModel input, CancellationToken cancellationToken = default);

    Task<AdminOperationResult<Club>> UpdateAsync(int id, ClubInputModel input, CancellationToken cancellationToken = default);

    Task<AdminOperationResult<Club>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
