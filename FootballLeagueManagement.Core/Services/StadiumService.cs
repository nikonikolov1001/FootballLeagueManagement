using FootballLeagueManagement.Core.Contracts;
using FootballLeagueManagement.Core.Entities;
using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Core.Services;

public class StadiumService : IStadiumService
{
    private readonly IRepository<Stadium> stadiums;

    public StadiumService(IRepository<Stadium> stadiums)
    {
        this.stadiums = stadiums;
    }

    public Task<IReadOnlyCollection<StadiumDto>> GetAllAsync()
    {
        var result = stadiums.All().OrderBy(s => s.Name).AsEnumerable().Select(ToDto).ToList().AsReadOnly();
        return Task.FromResult<IReadOnlyCollection<StadiumDto>>(result);
    }

    public async Task<StadiumDto?> GetByIdAsync(int id)
    {
        var stadium = await stadiums.FindAsync(id);
        return stadium is null ? null : ToDto(stadium);
    }

    public async Task<StadiumDto> CreateAsync(StadiumRequest request)
    {
        var stadium = new Stadium { Name = request.Name, City = request.City, Capacity = request.Capacity };
        await stadiums.AddAsync(stadium);
        await stadiums.SaveChangesAsync();
        return ToDto(stadium);
    }

    public async Task<bool> UpdateAsync(int id, StadiumRequest request)
    {
        var stadium = await stadiums.FindAsync(id);
        if (stadium is null)
        {
            return false;
        }

        stadium.Name = request.Name;
        stadium.City = request.City;
        stadium.Capacity = request.Capacity;
        await stadiums.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var stadium = await stadiums.FindAsync(id);
        if (stadium is null)
        {
            return false;
        }

        stadiums.Remove(stadium);
        await stadiums.SaveChangesAsync();
        return true;
    }

    private static StadiumDto ToDto(Stadium stadium)
        => new(stadium.Id, stadium.Name, stadium.City, stadium.Capacity);
}
