using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IUserListService
{
    IEnumerable<UserListDto> GetAllLists();
    UserListDto? GetListById(int id);
    IEnumerable<UserListDto> GetListsByUserId(int userId);
    UserListDto CreateList(CreateUserListDto dto, int userId);
    UserListDto? UpdateList(int id, UpdateUserListDto dto, int userId);
    bool DeleteList(int id, int userId);
    bool AddBeanToList(int listId, int beanId, int userId);
    bool RemoveBeanFromList(int listId, int beanId, int userId);
    IEnumerable<BeanDto> GetBeansInList(int listId);
}