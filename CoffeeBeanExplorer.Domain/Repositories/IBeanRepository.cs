using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IBeanRepository
{
    Task<IEnumerable<Bean>> GetAllAsync();
    Task<Bean?> GetByIdAsync(int id);
    Task<Bean> AddAsync(Bean bean);
    Task<bool> UpdateAsync(Bean bean);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}