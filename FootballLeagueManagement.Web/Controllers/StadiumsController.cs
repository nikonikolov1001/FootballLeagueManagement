using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueManagement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StadiumsController : ControllerBase
{
    private readonly IStadiumService stadiumService;

    public StadiumsController(IStadiumService stadiumService)
    {
        this.stadiumService = stadiumService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await stadiumService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var stadium = await stadiumService.GetByIdAsync(id);
        return stadium is null ? NotFound() : Ok(stadium);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPost]
    public async Task<IActionResult> Create(StadiumRequest request)
    {
        var stadium = await stadiumService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = stadium.Id }, stadium);
    }

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, StadiumRequest request)
        => await stadiumService.UpdateAsync(id, request) ? NoContent() : NotFound();

    [Authorize(Roles = DatabaseSeeder.AdministratorRole)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await stadiumService.DeleteAsync(id) ? NoContent() : NotFound();
}
