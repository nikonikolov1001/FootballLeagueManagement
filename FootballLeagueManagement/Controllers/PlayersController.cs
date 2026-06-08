using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All([FromQuery] int? clubId, CancellationToken cancellationToken)
    {
        var query = dbContext.Players.Include(player => player.Club).AsNoTracking();

        if (clubId.HasValue)
        {
            query = query.Where(player => player.ClubId == clubId.Value);
        }

        return Ok(await query.OrderBy(player => player.FullName).ToListAsync(cancellationToken));
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
        var player = new Player
        {
            FullName = input.FullName,
            Position = input.Position,
            ShirtNumber = input.ShirtNumber,
            ClubId = input.ClubId
        };

        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(ById), new { id = player.Id }, player);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PlayerInputModel input, CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.FirstOrDefaultAsync(player => player.Id == id, cancellationToken);
        if (player is null)
        {
            return NotFound();
        }

        player.FullName = input.FullName;
        player.Position = input.Position;
        player.ShirtNumber = input.ShirtNumber;
        player.ClubId = input.ClubId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.FirstOrDefaultAsync(player => player.Id == id, cancellationToken);
        if (player is null)
        {
            return NotFound();
        }

        dbContext.Players.Remove(player);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
