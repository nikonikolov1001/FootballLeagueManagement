namespace FootballLeagueManagement.Core.Contracts;

public interface IRepository<T>
    where T : class
{
    IQueryable<T> All();
    Task<T?> FindAsync(int id);
    Task AddAsync(T entity);
    void Remove(T entity);
    Task SaveChangesAsync();
}
