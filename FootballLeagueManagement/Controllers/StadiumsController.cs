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
public class StadiumsController(ApplicationDbContext dbContext, IConfiguration configuration) : ControllerBase
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
        var stadium = new Stadium { Name = input.Name, City = input.City, Capacity = input.Capacity };
        dbContext.Stadiums.Add(stadium);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(ById), new { id = stadium.Id }, stadium);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, StadiumInputModel input, CancellationToken cancellationToken)
    {
        var stadium = await dbContext.Stadiums.FirstOrDefaultAsync(stadium => stadium.Id == id, cancellationToken);
        if (stadium is null)
        {
            return NotFound();
        }

        stadium.Name = input.Name;
        stadium.City = input.City;
        stadium.Capacity = input.Capacity;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var stadium = await dbContext.Stadiums.FirstOrDefaultAsync(stadium => stadium.Id == id, cancellationToken);
        if (stadium is null)
        {
            return NotFound();
        }

        try
        {
            dbContext.Stadiums.Remove(stadium);
            await dbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict("This stadium cannot be deleted because it is assigned to a club.");
        }
    }
}
