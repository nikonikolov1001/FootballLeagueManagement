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
public class StadiumsController(
    ApplicationDbContext dbContext,
    IConfiguration configuration,
    IStadiumAdminService stadiumAdminService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All(CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            return Ok(DemoLeagueData.StadiumResponses());
        }

        try
        {
            return Ok(await dbContext.Stadiums.AsNoTracking().OrderBy(stadium => stadium.Name).ToListAsync(cancellationToken));
        }
        catch
        {
            return Ok(DemoLeagueData.StadiumResponses());
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ById(int id, CancellationToken cancellationToken)
    {
        var stadium = await dbContext.Stadiums.AsNoTracking().FirstOrDefaultAsync(stadium => stadium.Id == id, cancellationToken);
        return stadium is null ? NotFound() : Ok(stadium);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(StadiumInputModel input, CancellationToken cancellationToken)
    {
        var result = await stadiumAdminService.CreateAsync(input, cancellationToken);
        return ToActionResult(result, stadium => CreatedAtAction(nameof(ById), new { id = stadium.Id }, stadium));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, StadiumInputModel input, CancellationToken cancellationToken)
    {
        var result = await stadiumAdminService.UpdateAsync(id, input, cancellationToken);
        return ToActionResult(result, _ => NoContent());
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await stadiumAdminService.DeleteAsync(id, cancellationToken);
        return ToActionResult(result, _ => NoContent());
    }

    private IActionResult ToActionResult(AdminOperationResult<Stadium> result, Func<Stadium, IActionResult> onSuccess)
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
