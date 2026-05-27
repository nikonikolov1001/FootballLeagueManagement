using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueManagement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService teamService;

    public TeamsController(ITeamService teamService)
    {
        this.teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
        => Ok(await teamService.GetAllAsync(search));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var team = await teamService.GetByIdAsync(id);
        return team is null ? NotFound() : Ok(team);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPost]
    public async Task<IActionResult> Create(TeamRequest request)
    {
        var team = await teamService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TeamRequest request)
        => await teamService.UpdateAsync(id, request) ? NoContent() : NotFound();

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await teamService.DeleteAsync(id) ? NoContent() : NotFound();
}
