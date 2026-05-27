using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Core.Contracts;

public interface ITeamService
{
    Task<IReadOnlyCollection<TeamDto>> GetAllAsync(string? search = null);
    Task<TeamDto?> GetByIdAsync(int id);
    Task<TeamDto> CreateAsync(TeamRequest request);
    Task<bool> UpdateAsync(int id, TeamRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface IPlayerService
{
    Task<IReadOnlyCollection<PlayerDto>> GetAllAsync(int? teamId = null, string? search = null);
    Task<PlayerDto?> GetByIdAsync(int id);
    Task<PlayerDto> CreateAsync(PlayerRequest request);
    Task<bool> UpdateAsync(int id, PlayerRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface IStadiumService
{
    Task<IReadOnlyCollection<StadiumDto>> GetAllAsync();
    Task<StadiumDto?> GetByIdAsync(int id);
    Task<StadiumDto> CreateAsync(StadiumRequest request);
    Task<bool> UpdateAsync(int id, StadiumRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface IMatchService
{
    Task<IReadOnlyCollection<MatchDto>> GetAllAsync(bool? finished = null);
    Task<MatchDto?> GetByIdAsync(int id);
    Task<MatchDto> CreateAsync(MatchRequest request);
    Task<bool> UpdateAsync(int id, MatchRequest request);
    Task<bool> SetResultAsync(int id, MatchResultRequest request);
    Task<bool> DeleteAsync(int id);
}
