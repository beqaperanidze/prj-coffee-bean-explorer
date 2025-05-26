using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IOriginService
{
    Task<IEnumerable<OriginDto>> GetAllOriginsAsync();
    Task<OriginDto?> GetOriginByIdAsync(int id);
    Task<OriginDto> CreateOriginAsync(CreateOriginDto dto);
    Task<bool> UpdateOriginAsync(int id, UpdateOriginDto dto);
    Task<bool> DeleteOriginAsync(int id);
}