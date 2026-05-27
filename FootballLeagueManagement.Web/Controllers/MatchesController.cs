using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueManagement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService matchService;

    public MatchesController(IMatchService matchService)
    {
        this.matchService = matchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? finished)
        => Ok(await matchService.GetAllAsync(finished));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var match = await matchService.GetByIdAsync(id);
        return match is null ? NotFound() : Ok(match);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPost]
    public async Task<IActionResult> Create(MatchRequest request)
    {
        var match = await matchService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = match.Id }, match);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MatchRequest request)
        => await matchService.UpdateAsync(id, request) ? NoContent() : NotFound();

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPatch("{id:int}/result")]
    public async Task<IActionResult> SetResult(int id, MatchResultRequest request)
        => await matchService.SetResultAsync(id, request) ? NoContent() : NotFound();

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await matchService.DeleteAsync(id) ? NoContent() : NotFound();
}
