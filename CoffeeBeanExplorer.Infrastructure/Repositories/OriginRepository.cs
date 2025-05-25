using System;
using System.Collections.Generic;
using System.Linq;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class OriginRepository : IOriginRepository
{
    private static readonly List<Origin> _origins = [];
    private static int _nextId = 1;

    public IEnumerable<Origin> GetAll() => _origins;

    public Origin? GetById(int id)
    {
        return _origins.FirstOrDefault(o => o.Id == id);
    }

    public Origin Add(Origin origin)
    {
        origin.Id = _nextId++;
        origin.CreatedAt = DateTime.UtcNow;
        origin.UpdatedAt = DateTime.UtcNow;
        _origins.Add(origin);
        return origin;
    }

    public bool Update(Origin origin)
    {
        var existingOrigin = _origins.FirstOrDefault(o => o.Id == origin.Id);
        if (existingOrigin is null) return false;

        existingOrigin.Country = origin.Country;
        existingOrigin.Region = origin.Region;
        existingOrigin.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public bool Delete(int id)
    {
        var origin = _origins.FirstOrDefault(o => o.Id == id);
        return origin is not null && _origins.Remove(origin);
    }
}