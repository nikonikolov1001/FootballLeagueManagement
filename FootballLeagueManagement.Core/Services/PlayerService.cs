using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Entities;
using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Core.Services;

public class PlayerService : IPlayerService
{
    private readonly IRepository<Player> players;

    public PlayerService(IRepository<Player> players)
    {
        this.players = players;
    }

    public Task<IReadOnlyCollection<PlayerDto>> GetAllAsync(int? teamId = null, string? search = null)
    {
        var query = players.All();

        if (teamId.HasValue)
        {
            query = query.Where(p => p.TeamId == teamId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search));
        }

        var result = query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).AsEnumerable().Select(ToDto).ToList().AsReadOnly();
        return Task.FromResult<IReadOnlyCollection<PlayerDto>>(result);
    }

    public async Task<PlayerDto?> GetByIdAsync(int id)
    {
        var player = await players.FindAsync(id);
        return player is null ? null : ToDto(player);
    }

    public async Task<PlayerDto> CreateAsync(PlayerRequest request)
    {
        var player = new Player
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = request.Position,
            ShirtNumber = request.ShirtNumber,
            Nationality = request.Nationality,
            TeamId = request.TeamId
        };

        await players.AddAsync(player);
        await players.SaveChangesAsync();
        return ToDto(player);
    }

    public async Task<bool> UpdateAsync(int id, PlayerRequest request)
    {
        var player = await players.FindAsync(id);
        if (player is null)
        {
            return false;
        }

        player.FirstName = request.FirstName;
        player.LastName = request.LastName;
        player.Position = request.Position;
        player.ShirtNumber = request.ShirtNumber;
        player.Nationality = request.Nationality;
        player.TeamId = request.TeamId;
        await players.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var player = await players.FindAsync(id);
        if (player is null)
        {
            return false;
        }

        players.Remove(player);
        await players.SaveChangesAsync();
        return true;
    }

    private static PlayerDto ToDto(Player player)
        => new(player.Id, player.FirstName, player.LastName, player.Position, player.ShirtNumber, player.Nationality, player.TeamId, player.Team?.Name);
}
