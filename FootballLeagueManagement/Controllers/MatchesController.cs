using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Core.Services;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController(ApplicationDbContext dbContext, IMatchService matchService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All([FromQuery] bool? played, CancellationToken cancellationToken)
    {
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

        return Ok(await query.OrderBy(match => match.KickoffUtc).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ById(int id, CancellationToken cancellationToken)
    {
        var match = await dbContext.Matches
            .Include(match => match.HomeClub)
            .Include(match => match.AwayClub)
            .AsNoTracking()
            .FirstOrDefaultAsync(match => match.Id == id, cancellationToken);

        return match is null ? NotFound() : Ok(match);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(MatchInputModel input, CancellationToken cancellationToken)
    {
        if (input.HomeClubId == input.AwayClubId)
        {
            return BadRequest("Home and away clubs must be different.");
        }

        var match = new Match
        {
            HomeClubId = input.HomeClubId,
            AwayClubId = input.AwayClubId,
            KickoffUtc = input.KickoffUtc
        };

        dbContext.Matches.Add(match);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(ById), new { id = match.Id }, match);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MatchInputModel input, CancellationToken cancellationToken)
    {
        if (input.HomeClubId == input.AwayClubId)
        {
            return BadRequest("Home and away clubs must be different.");
        }

        var match = await dbContext.Matches.FirstOrDefaultAsync(match => match.Id == id, cancellationToken);
        if (match is null)
        {
            return NotFound();
        }

        match.HomeClubId = input.HomeClubId;
        match.AwayClubId = input.AwayClubId;
        match.KickoffUtc = input.KickoffUtc;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
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
        var match = await dbContext.Matches.FirstOrDefaultAsync(match => match.Id == id, cancellationToken);
        if (match is null)
        {
            return NotFound();
        }

        dbContext.Matches.Remove(match);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
