using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IOriginRepository
{
    Task<IEnumerable<Origin>> GetAllAsync();
    Task<Origin?> GetByIdAsync(int id);
    Task<Origin> AddAsync(Origin origin);
    Task<bool> UpdateAsync(Origin origin);
    Task<bool> DeleteAsync(int id);
}