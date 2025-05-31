using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class OriginService(IOriginRepository repository, IMapper mapper) : IOriginService
{
    public async Task<IEnumerable<OriginDto>> GetAllOriginsAsync()
    {
        var origins = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<OriginDto>>(origins);
    }

    public async Task<OriginDto?> GetOriginByIdAsync(int id)
    {
        var origin = await repository.GetByIdAsync(id);
        return origin != null ? mapper.Map<OriginDto>(origin) : null;
    }

    public async Task<OriginDto> CreateOriginAsync(CreateOriginDto dto)
    {
        var origin = new Origin
        {
            Country = dto.Country,
            Region = dto.Region
        };

        var addedOrigin = await repository.AddAsync(origin);
        return mapper.Map<OriginDto>(addedOrigin);
    }

    public async Task<bool> UpdateOriginAsync(int id, UpdateOriginDto dto)
    {
        var origin = await repository.GetByIdAsync(id);
        if (origin is null) return false;

        origin.Country = dto.Country;
        origin.Region = dto.Region;

        return await repository.UpdateAsync(origin);
    }

    public async Task<bool> DeleteOriginAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }
}
