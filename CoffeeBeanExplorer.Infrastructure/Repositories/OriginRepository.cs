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

    public Task<IEnumerable<Origin>> GetAllAsync() => Task.FromResult<IEnumerable<Origin>>(_origins);

    public Task<Origin?> GetByIdAsync(int id)
    {
        return Task.FromResult(_origins.FirstOrDefault(o => o.Id == id));
    }

    public Task<Origin> AddAsync(Origin origin)
    {
        origin.Id = _nextId++;
        origin.CreatedAt = DateTime.UtcNow;
        origin.UpdatedAt = DateTime.UtcNow;
        _origins.Add(origin);
        return Task.FromResult(origin);
    }

    public Task<bool> UpdateAsync(Origin origin)
    {
        var existingOrigin = _origins.FirstOrDefault(o => o.Id == origin.Id);
        if (existingOrigin is null) return Task.FromResult(false);

        existingOrigin.Country = origin.Country;
        existingOrigin.Region = origin.Region;
        existingOrigin.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var origin = _origins.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(origin is not null && _origins.Remove(origin));
    }
}