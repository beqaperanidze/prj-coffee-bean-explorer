using System;
using System.Collections.Generic;
using System.Linq;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;


namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class BeanRepository : IBeanRepository
{
    private static readonly List<Bean> _beans = [];
    private static int _nextId = 1;

    public Task<IEnumerable<Bean>> GetAllAsync() => Task.FromResult<IEnumerable<Bean>>(_beans);

    public Task<Bean?> GetByIdAsync(int id)
    {
        return Task.FromResult(_beans.FirstOrDefault(b => b.Id == id));
    }

    public Task<Bean> AddAsync(Bean bean)
    {
        bean.Id = _nextId++;
        bean.CreatedAt = DateTime.UtcNow;
        bean.UpdatedAt = DateTime.UtcNow;
        _beans.Add(bean);
        return Task.FromResult(bean);
    }

    public Task<bool> UpdateAsync(Bean bean)
    {
        var existingBean = _beans.FirstOrDefault(b => b.Id == bean.Id);
        if (existingBean is null) return Task.FromResult(false);

        existingBean.Name = bean.Name;
        existingBean.OriginId = bean.OriginId;
        existingBean.RoastLevel = bean.RoastLevel;
        existingBean.Description = bean.Description;
        existingBean.Price = bean.Price;
        existingBean.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var bean = _beans.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(bean is not null && _beans.Remove(bean));
    }
}