using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using FootballLeagueManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(
    ApplicationDbContext dbContext,
    IConfiguration configuration,
    IPlayerAdminService playerAdminService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All([FromQuery] int? clubId, CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            return Ok(DemoLeagueData.PlayerResponses(clubId));
        }

        var query = dbContext.Players.Include(player => player.Club).AsNoTracking();

        if (clubId.HasValue)
        {
            query = query.Where(player => player.ClubId == clubId.Value);
        }

        try
        {
            var players = await query.OrderBy(player => player.FullName).ToListAsync(cancellationToken);

            return Ok(players.Select(player => new
            {
                player.Id,
                player.FullName,
                player.Position,
                player.ShirtNumber,
                player.ClubId,
                Club = player.Club?.Name,
                Stats = DemoLeagueData.BuildPlayerStats(player.FullName)
            }));
        }
        catch
        {
            return Ok(DemoLeagueData.PlayerResponses(clubId));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ById(int id, CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.Include(player => player.Club).AsNoTracking().FirstOrDefaultAsync(player => player.Id == id, cancellationToken);
        return player is null ? NotFound() : Ok(player);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(PlayerInputModel input, CancellationToken cancellationToken)
    {
        var result = await playerAdminService.CreateAsync(input, cancellationToken);
        return ToActionResult(result, player => CreatedAtAction(nameof(ById), new { id = player.Id }, player));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PlayerInputModel input, CancellationToken cancellationToken)
    {
        var result = await playerAdminService.UpdateAsync(id, input, cancellationToken);
        return ToActionResult(result, _ => NoContent());
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await playerAdminService.DeleteAsync(id, cancellationToken);
        return ToActionResult(result, _ => NoContent());
    }

    private IActionResult ToActionResult(AdminOperationResult<Player> result, Func<Player, IActionResult> onSuccess)
    {
        return result.Status switch
        {
            AdminOperationStatus.Success => onSuccess(result.Value!),
            AdminOperationStatus.NotFound => NotFound(result.Message),
            AdminOperationStatus.BadRequest => BadRequest(result.Message),
            AdminOperationStatus.Conflict => Conflict(result.Message),
            _ => BadRequest()
        };
    }
}
