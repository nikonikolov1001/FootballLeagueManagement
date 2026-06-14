using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Core.Services;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using FootballLeagueManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController(ApplicationDbContext dbContext, IMatchService matchService, IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All([FromQuery] bool? played, CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            return Ok(DemoLeagueData.MatchResponses(played));
        }

        var query = dbContext.Matches
            .Include(match => match.HomeClub)
            .Include(match => match.AwayClub)
            .AsNoTracking();

        if (played.HasValue)
        {
            query = played.Value
                ? query.Where(match => match.HomeGoals.HasValue && match.AwayGoals.HasValue)
                : query.Where(match => !match.HomeGoals.HasValue || !match.AwayGoals.HasValue);
        }

        try
        {
            return Ok(await query.OrderBy(match => match.KickoffUtc).ToListAsync(cancellationToken));
        }
        catch
        {
            return Ok(DemoLeagueData.MatchResponses(played));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ById(int id, CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            var demoMatch = DemoLeagueData.MatchResponses(null).FirstOrDefault(match => (int)match.GetType().GetProperty("Id")!.GetValue(match)! == id);
            return demoMatch is null ? NotFound() : Ok(demoMatch);
        }

        Match? match;
        try
        {
            match = await dbContext.Matches
                .Include(match => match.HomeClub)
                .Include(match => match.AwayClub)
                .AsNoTracking()
                .FirstOrDefaultAsync(match => match.Id == id, cancellationToken);
        }
        catch
        {
            var demoMatch = DemoLeagueData.MatchResponses(null).FirstOrDefault(match => (int)match.GetType().GetProperty("Id")!.GetValue(match)! == id);
            return demoMatch is null ? NotFound() : Ok(demoMatch);
        }

        return match is null ? NotFound() : Ok(match);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(MatchInputModel input, CancellationToken cancellationToken)
    {
        try
        {
            var match = await matchService.CreateAsync(input.HomeClubId, input.AwayClubId, input.KickoffUtc, cancellationToken);
            return CreatedAtAction(nameof(ById), new { id = match.Id }, match);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MatchInputModel input, CancellationToken cancellationToken)
    {
        try
        {
            await matchService.UpdateAsync(id, input.HomeClubId, input.AwayClubId, input.KickoffUtc, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch("{id:int}/result")]
    public async Task<IActionResult> RecordResult(int id, ResultInputModel input, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await matchService.RecordResultAsync(id, input.HomeGoals, input.AwayGoals, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await matchService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }
}
