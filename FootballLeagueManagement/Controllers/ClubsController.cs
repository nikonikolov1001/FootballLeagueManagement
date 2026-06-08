using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClubsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = dbContext.Clubs.Include(club => club.Stadium).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(club => club.Name.Contains(search) || club.City.Contains(search));
        }

        return Ok(await query.OrderBy(club => club.Name).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ById(int id, CancellationToken cancellationToken)
    {
        var club = await dbContext.Clubs.Include(club => club.Stadium).AsNoTracking().FirstOrDefaultAsync(club => club.Id == id, cancellationToken);
        return club is null ? NotFound() : Ok(club);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(ClubInputModel input, CancellationToken cancellationToken)
    {
        var club = new Club
        {
            Name = input.Name,
            ShortCode = input.ShortCode,
            City = input.City,
            FoundedYear = input.FoundedYear,
            StadiumId = input.StadiumId
        };

        dbContext.Clubs.Add(club);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(ById), new { id = club.Id }, club);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ClubInputModel input, CancellationToken cancellationToken)
    {
        var club = await dbContext.Clubs.FirstOrDefaultAsync(club => club.Id == id, cancellationToken);
        if (club is null)
        {
            return NotFound();
        }

        club.Name = input.Name;
        club.ShortCode = input.ShortCode;
        club.City = input.City;
        club.FoundedYear = input.FoundedYear;
        club.StadiumId = input.StadiumId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var club = await dbContext.Clubs.FirstOrDefaultAsync(club => club.Id == id, cancellationToken);
        if (club is null)
        {
            return NotFound();
        }

        dbContext.Clubs.Remove(club);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
