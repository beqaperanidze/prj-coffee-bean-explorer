using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IUserRepository
{
    IEnumerable<User> GetAll();
    User? GetById(int id);
    User? GetByUsername(string username);
    User? GetByEmail(string email);
    User Add(User user);
    bool Update(User user);
    bool Delete(int id);
}