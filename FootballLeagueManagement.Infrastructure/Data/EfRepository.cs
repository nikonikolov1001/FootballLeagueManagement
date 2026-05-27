using FootballLeagueManagement.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Infrastructure.Data;

public class EfRepository<T> : IRepository<T>
    where T : class
{
    private readonly ApplicationDbContext dbContext;

    public EfRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<T> All()
        => dbContext.Set<T>().AsNoTracking();

    public Task<T?> FindAsync(int id)
        => dbContext.Set<T>().FindAsync(id).AsTask();

    public Task AddAsync(T entity)
        => dbContext.Set<T>().AddAsync(entity).AsTask();

    public void Remove(T entity)
        => dbContext.Set<T>().Remove(entity);

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
