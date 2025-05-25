using System;
using System.Collections.Generic;
using System.Linq;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class UserListRepository : IUserListRepository
{
    private static readonly List<UserList> _lists = [];
    private static readonly List<ListItem> _listItems = [];
    private static int _nextId = 1;

    public IEnumerable<UserList> GetAll() => _lists;

    public UserList? GetById(int id)
    {
        return _lists.FirstOrDefault(l => l.Id == id);
    }

    public IEnumerable<UserList> GetByUserId(int userId)
    {
        return _lists.Where(l => l.UserId == userId);
    }

    public UserList Add(UserList list)
    {
        list.Id = _nextId++;
        list.CreatedAt = DateTime.UtcNow;
        list.UpdatedAt = DateTime.UtcNow;
        _lists.Add(list);
        return list;
    }

    public bool Update(UserList list)
    {
        var existingList = _lists.FirstOrDefault(l => l.Id == list.Id);
        if (existingList is null) return false;

        existingList.Name = list.Name;
        existingList.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public bool Delete(int id)
    {
        var list = _lists.FirstOrDefault(l => l.Id == id);
        if (list is null) return false;

        _listItems.RemoveAll(li => li.ListId == id);

        return _lists.Remove(list);
    }

    public bool AddBeanToList(int listId, int beanId)
    {
        if (_listItems.Any(li => li.ListId == listId && li.BeanId == beanId))
        {
            return false;
        }

        var list = GetById(listId);
        if (list == null) return false;

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

        return true;
    }

    public bool RemoveBeanFromList(int listId, int beanId)
    {
        var listItem = _listItems.FirstOrDefault(li => li.ListId == listId && li.BeanId == beanId);
        if (listItem is null) return false;

        var list = GetById(listId);
        list?.Items.Remove(listItem);

        return _listItems.Remove(listItem);
    }

    public IEnumerable<Bean> GetBeansInList(int listId)
    {
        return _listItems
            .Where(li => li.ListId == listId)
            .Select(li => li.Bean ?? new Bean { Id = li.BeanId });
    }
}