using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IUserListRepository
{
    IEnumerable<UserList> GetAll();
    UserList? GetById(int id);
    IEnumerable<UserList> GetByUserId(int userId);
    UserList Add(UserList list);
    bool Update(UserList list);
    bool Delete(int id);
    bool AddBeanToList(int listId, int beanId);
    bool RemoveBeanFromList(int listId, int beanId);
    IEnumerable<Bean> GetBeansInList(int listId);
}