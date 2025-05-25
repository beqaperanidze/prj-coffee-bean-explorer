using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IOriginService
{
    IEnumerable<OriginDto> GetAllOrigins();
    OriginDto? GetOriginById(int id);
    OriginDto CreateOrigin(CreateOriginDto dto);
    bool UpdateOrigin(int id, UpdateOriginDto dto);
    bool DeleteOrigin(int id);
}