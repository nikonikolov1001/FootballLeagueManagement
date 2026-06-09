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
        var validation = await ValidateClubsAsync(input.HomeClubId, input.AwayClubId, cancellationToken);
        if (validation is not null)
        {
            return validation;
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
        var validation = await ValidateClubsAsync(input.HomeClubId, input.AwayClubId, cancellationToken);
        if (validation is not null)
        {
            return validation;
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

    private async Task<IActionResult?> ValidateClubsAsync(int homeClubId, int awayClubId, CancellationToken cancellationToken)
    {
        if (homeClubId == awayClubId)
        {
            return BadRequest("Home and away clubs must be different.");
        }

        var foundClubs = await dbContext.Clubs
            .CountAsync(club => club.Id == homeClubId || club.Id == awayClubId, cancellationToken);

        return foundClubs == 2 ? null : BadRequest("Both clubs must exist before creating a match.");
    }
}
