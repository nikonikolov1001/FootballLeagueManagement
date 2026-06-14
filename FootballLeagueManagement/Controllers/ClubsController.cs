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
public class ClubsController(
    ApplicationDbContext dbContext,
    IConfiguration configuration,
    IClubAdminService clubAdminService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All([FromQuery] string? search, CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            return Ok(DemoLeagueData.ClubResponses(search));
        }

        var query = dbContext.Clubs.Include(club => club.Stadium).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(club => club.Name.Contains(search) || club.City.Contains(search));
        }

        try
        {
            return Ok(await query.OrderBy(club => club.Name).ToListAsync(cancellationToken));
        }
        catch
        {
            return Ok(DemoLeagueData.ClubResponses(search).OrderBy(club => club.GetType().GetProperty("Name")?.GetValue(club)));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ById(int id, CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            var demoClub = DemoLeagueData.ClubResponses(null).FirstOrDefault(club => (int)club.GetType().GetProperty("Id")!.GetValue(club)! == id);
            return demoClub is null ? NotFound() : Ok(demoClub);
        }

        Club? club;
        try
        {
            club = await dbContext.Clubs.Include(club => club.Stadium).AsNoTracking().FirstOrDefaultAsync(club => club.Id == id, cancellationToken);
        }
        catch
        {
            var demoClub = DemoLeagueData.ClubResponses(null).FirstOrDefault(club => (int)club.GetType().GetProperty("Id")!.GetValue(club)! == id);
            return demoClub is null ? NotFound() : Ok(demoClub);
        }

        return club is null ? NotFound() : Ok(club);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(ClubInputModel input, CancellationToken cancellationToken)
    {
        var result = await clubAdminService.CreateAsync(input, cancellationToken);
        return ToActionResult(result, club => CreatedAtAction(nameof(ById), new { id = club.Id }, club));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ClubInputModel input, CancellationToken cancellationToken)
    {
        var result = await clubAdminService.UpdateAsync(id, input, cancellationToken);
        return ToActionResult(result, _ => NoContent());
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await clubAdminService.DeleteAsync(id, cancellationToken);
        return ToActionResult(result, _ => NoContent());
    }

    private IActionResult ToActionResult(AdminOperationResult<Club> result, Func<Club, IActionResult> onSuccess)
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
