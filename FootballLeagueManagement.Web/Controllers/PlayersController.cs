using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueManagement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService playerService;

    public PlayersController(IPlayerService playerService)
    {
        this.playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? teamId, [FromQuery] string? search)
        => Ok(await playerService.GetAllAsync(teamId, search));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var player = await playerService.GetByIdAsync(id);
        return player is null ? NotFound() : Ok(player);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPost]
    public async Task<IActionResult> Create(PlayerRequest request)
    {
        var player = await playerService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = player.Id }, player);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PlayerRequest request)
        => await playerService.UpdateAsync(id, request) ? NoContent() : NotFound();

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await playerService.DeleteAsync(id) ? NoContent() : NotFound();
}
