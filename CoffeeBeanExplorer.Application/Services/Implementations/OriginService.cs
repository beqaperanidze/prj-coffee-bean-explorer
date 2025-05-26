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

    public async Task<IEnumerable<OriginDto>> GetAllOriginsAsync()
    {
        var origins = await _repository.GetAllAsync();
        return origins.Select(MapToDto);
    }

    public async Task<OriginDto?> GetOriginByIdAsync(int id)
    {
        var origin = await _repository.GetByIdAsync(id);
        return origin != null ? MapToDto(origin) : null;
    }

    public async Task<OriginDto> CreateOriginAsync(CreateOriginDto dto)
    {
        var origin = new Origin
        {
            Country = dto.Country,
            Region = dto.Region
        };

        var addedOrigin = await _repository.AddAsync(origin);
        return MapToDto(addedOrigin);
    }

    public async Task<bool> UpdateOriginAsync(int id, UpdateOriginDto dto)
    {
        var origin = await _repository.GetByIdAsync(id);
        if (origin is null) return false;

        origin.Country = dto.Country;
        origin.Region = dto.Region;

        return await _repository.UpdateAsync(origin);
    }

    public async Task<bool> DeleteOriginAsync(int id)
    {
        return await _repository.DeleteAsync(id);
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