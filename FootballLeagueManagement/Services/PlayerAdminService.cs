using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Services;

public class PlayerAdminService(ApplicationDbContext dbContext) : IPlayerAdminService
{
    public async Task<AdminOperationResult<Player>> CreateAsync(PlayerInputModel input, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(input, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        var player = new Player
        {
            FullName = input.FullName,
            Position = input.Position,
            ShirtNumber = input.ShirtNumber,
            ClubId = input.ClubId
        };

        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Player>.Success(player);
    }

    public async Task<AdminOperationResult<Player>> UpdateAsync(int id, PlayerInputModel input, CancellationToken cancellationToken = default)
    {
        var player = await dbContext.Players.FirstOrDefaultAsync(player => player.Id == id, cancellationToken);
        if (player is null)
        {
            return AdminOperationResult<Player>.NotFound();
        }

        var validation = await ValidateAsync(input, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        player.FullName = input.FullName;
        player.Position = input.Position;
        player.ShirtNumber = input.ShirtNumber;
        player.ClubId = input.ClubId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Player>.Success(player);
    }

    public async Task<AdminOperationResult<Player>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var player = await dbContext.Players.FirstOrDefaultAsync(player => player.Id == id, cancellationToken);
        if (player is null)
        {
            return AdminOperationResult<Player>.NotFound();
        }

        dbContext.Players.Remove(player);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Player>.Success(player);
    }

    private async Task<AdminOperationResult<Player>?> ValidateAsync(PlayerInputModel input, CancellationToken cancellationToken)
    {
        return await dbContext.Clubs.AnyAsync(club => club.Id == input.ClubId, cancellationToken)
            ? null
            : AdminOperationResult<Player>.BadRequest("The selected club does not exist.");
    }
}
