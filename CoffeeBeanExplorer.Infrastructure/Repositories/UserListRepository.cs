using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class UserListRepository : IUserListRepository
{
    private static readonly List<UserList> _lists = [];
    private static readonly List<ListItem> _listItems = [];
    private static int _nextId = 1;

    public Task<IEnumerable<UserList>> GetAllAsync() => Task.FromResult<IEnumerable<UserList>>(_lists);

    public Task<UserList?> GetByIdAsync(int id)
    {
        return Task.FromResult(_lists.FirstOrDefault(l => l.Id == id));
    }

    public Task<IEnumerable<UserList>> GetByUserIdAsync(int userId)
    {
        return Task.FromResult(_lists.Where(l => l.UserId == userId));
    }

    public Task<UserList> AddAsync(UserList list)
    {
        list.Id = _nextId++;
        list.CreatedAt = DateTime.UtcNow;
        list.UpdatedAt = DateTime.UtcNow;
        _lists.Add(list);
        return Task.FromResult(list);
    }

    public Task<bool> UpdateAsync(UserList list)
    {
        var existingList = _lists.FirstOrDefault(l => l.Id == list.Id);
        if (existingList is null) return Task.FromResult(false);

        existingList.Name = list.Name;
        existingList.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var list = _lists.FirstOrDefault(l => l.Id == id);
        if (list is null) return Task.FromResult(false);

        _listItems.RemoveAll(li => li.ListId == id);

        return Task.FromResult(_lists.Remove(list));
    }

    public Task<bool> AddBeanToListAsync(int listId, int beanId)
    {
        if (_listItems.Any(li => li.ListId == listId && li.BeanId == beanId))
        {
            return Task.FromResult(false);
        }

        var list = _lists.FirstOrDefault(l => l.Id == listId);
        if (list == null) return Task.FromResult(false);

        var listItem = new ListItem
        {
            ListId = listId,
            BeanId = beanId,
            CreatedAt = DateTime.UtcNow,
            List = list,
            Bean = new Bean { Id = beanId }
        };

        _listItems.Add(listItem);
        list.Items.Add(listItem);

        return Task.FromResult(true);
    }

    public Task<bool> RemoveBeanFromListAsync(int listId, int beanId)
    {
        var listItem = _listItems.FirstOrDefault(li => li.ListId == listId && li.BeanId == beanId);
        if (listItem is null) return Task.FromResult(false);

        var list = _lists.FirstOrDefault(l => l.Id == listId);
        list?.Items.Remove(listItem);

        return Task.FromResult(_listItems.Remove(listItem));
    }

    public Task<IEnumerable<Bean>> GetBeansInListAsync(int listId)
    {
        return Task.FromResult(_listItems
            .Where(li => li.ListId == listId)
            .Select(li => li.Bean ?? new Bean { Id = li.BeanId }));
    }
}