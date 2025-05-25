using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class OriginService : IOriginService
{
    private readonly IOriginRepository _repository;

    public OriginService(IOriginRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<OriginDto> GetAllOrigins()
    {
        return _repository.GetAll().Select(MapToDto);
    }

    public OriginDto? GetOriginById(int id)
    {
        var origin = _repository.GetById(id);
        return origin != null ? MapToDto(origin) : null;
    }

    public OriginDto CreateOrigin(CreateOriginDto dto)
    {
        var origin = new Origin
        {
            Country = dto.Country,
            Region = dto.Region
        };

        var addedOrigin = _repository.Add(origin);
        return MapToDto(addedOrigin);
    }

    public bool UpdateOrigin(int id, UpdateOriginDto dto)
    {
        var origin = _repository.GetById(id);
        if (origin is null) return false;

        origin.Country = dto.Country;
        origin.Region = dto.Region;

        return _repository.Update(origin);
    }

    public bool DeleteOrigin(int id)
    {
        return _repository.Delete(id);
    }

    private static OriginDto MapToDto(Origin origin) => new OriginDto
    {
        Id = origin.Id,
        Country = origin.Country,
        Region = origin.Region,
        CreatedAt = origin.CreatedAt,
        UpdatedAt = origin.UpdatedAt
    };
}