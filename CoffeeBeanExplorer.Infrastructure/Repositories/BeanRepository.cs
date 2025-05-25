using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class BeanRepository : IBeanRepository
{
    private static readonly List<Bean> _beans = [];
    private static int _nextId = 1;

    public IEnumerable<Bean> GetAll() => _beans;

    public Bean? GetById(int id)
    {
        return _beans.FirstOrDefault(b => b.Id == id);
    }

    public Bean Add(Bean bean)
    {
        bean.Id = _nextId++;
        bean.CreatedAt = DateTime.UtcNow;
        bean.UpdatedAt = DateTime.UtcNow;
        _beans.Add(bean);
        return bean;
    }

    public bool Update(Bean bean)
    {
        var existingBean = _beans.FirstOrDefault(b => b.Id == bean.Id);
        if (existingBean is null) return false;

        existingBean.Name = bean.Name;
        existingBean.OriginId = bean.OriginId;
        existingBean.RoastLevel = bean.RoastLevel;
        existingBean.Description = bean.Description;
        existingBean.Price = bean.Price;
        existingBean.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public bool Delete(int id)
    {
        var bean = _beans.FirstOrDefault(b => b.Id == id);
        return bean is not null && _beans.Remove(bean);
    }
}