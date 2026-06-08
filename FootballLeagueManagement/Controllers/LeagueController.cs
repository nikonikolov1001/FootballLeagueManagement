using FootballLeagueManagement.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeagueController(ILeagueQueryService leagueQueryService) : ControllerBase
{
    [HttpGet("standings")]
    public async Task<IActionResult> Standings(CancellationToken cancellationToken) =>
        Ok(await leagueQueryService.GetStandingsAsync(cancellationToken));

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken) =>
        Ok(await leagueQueryService.GetSummaryAsync(cancellationToken));
}
