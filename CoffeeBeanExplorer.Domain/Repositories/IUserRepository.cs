using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<(bool UsernameExists, bool EmailExists)> CheckUserExistsAsync(string username, string email);
    Task<User> AddAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
