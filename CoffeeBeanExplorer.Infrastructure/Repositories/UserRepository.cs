using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private static readonly List<User> _users = [];
    private static int _nextId = 1;

    public IEnumerable<User> GetAll() => _users;

    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public User? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username);
    }

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }

    public User Add(User user)
    {
        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        _users.Add(user);
        return user;
    }

    public bool Update(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser is null) return false;

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public bool Delete(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return user is not null && _users.Remove(user);
    }
}