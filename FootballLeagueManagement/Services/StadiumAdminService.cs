using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Services;

public class StadiumAdminService(ApplicationDbContext dbContext) : IStadiumAdminService
{
    public async Task<AdminOperationResult<Stadium>> CreateAsync(StadiumInputModel input, CancellationToken cancellationToken = default)
    {
        var stadium = new Stadium { Name = input.Name, City = input.City, Capacity = input.Capacity };
        dbContext.Stadiums.Add(stadium);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Stadium>.Success(stadium);
    }

    public async Task<AdminOperationResult<Stadium>> UpdateAsync(int id, StadiumInputModel input, CancellationToken cancellationToken = default)
    {
        var stadium = await dbContext.Stadiums.FirstOrDefaultAsync(stadium => stadium.Id == id, cancellationToken);
        if (stadium is null)
        {
            return AdminOperationResult<Stadium>.NotFound();
        }

        stadium.Name = input.Name;
        stadium.City = input.City;
        stadium.Capacity = input.Capacity;
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Stadium>.Success(stadium);
    }

    public async Task<AdminOperationResult<Stadium>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var stadium = await dbContext.Stadiums.FirstOrDefaultAsync(stadium => stadium.Id == id, cancellationToken);
        if (stadium is null)
        {
            return AdminOperationResult<Stadium>.NotFound();
        }

        dbContext.Stadiums.Remove(stadium);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Stadium>.Success(stadium);
    }
}
