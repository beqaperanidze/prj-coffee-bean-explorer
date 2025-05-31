using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IUserListService
{
    Task<IEnumerable<UserListDto>> GetAllListsAsync();
    Task<UserListDto?> GetListByIdAsync(int id);
    Task<IEnumerable<UserListDto>> GetListsByUserIdAsync(int userId);
    Task<UserListDto> CreateListAsync(CreateUserListDto dto, int userId);
    Task<UserListDto?> UpdateListAsync(int id, UpdateUserListDto dto, int userId);
    Task<bool> DeleteListAsync(int id, int userId);
    Task<bool> AddBeanToListAsync(int listId, int beanId, int userId);
    Task<bool> RemoveBeanFromListAsync(int listId, int beanId, int userId);
}