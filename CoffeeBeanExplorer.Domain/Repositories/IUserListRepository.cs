using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IUserListRepository
{
    Task<IEnumerable<UserList>> GetAllAsync();
    Task<UserList?> GetByIdAsync(int id);
    Task<IEnumerable<UserList>> GetByUserIdAsync(int userId);
    Task<UserList> AddAsync(UserList list);
    Task<bool> UpdateAsync(UserList list);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddBeanToListAsync(int listId, int beanId);
    Task<bool> RemoveBeanFromListAsync(int listId, int beanId);
    Task<IEnumerable<Bean>> GetBeansInListAsync(int listId);
}