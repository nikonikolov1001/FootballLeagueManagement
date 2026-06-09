using FootballLeagueManagement.Core.Services;
using FootballLeagueManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeagueController(ILeagueQueryService leagueQueryService, IConfiguration configuration) : ControllerBase
{
    [HttpGet("standings")]
    public async Task<IActionResult> Standings(CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            return Ok(DemoLeagueData.Standings);
        }

        try
        {
            return Ok(await leagueQueryService.GetStandingsAsync(cancellationToken));
        }
        catch
        {
            return Ok(DemoLeagueData.Standings);
        }
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        if (configuration.GetValue<bool>("UseDemoData"))
        {
            return Ok(DemoLeagueData.Summary);
        }

        try
        {
            return Ok(await leagueQueryService.GetSummaryAsync(cancellationToken));
        }
        catch
        {
            return Ok(DemoLeagueData.Summary);
        }
    }
}
