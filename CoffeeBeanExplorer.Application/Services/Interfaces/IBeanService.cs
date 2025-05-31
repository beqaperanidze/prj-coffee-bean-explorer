using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IBeanService
{
    Task<IEnumerable<BeanDto>> GetAllBeansAsync();
    Task<BeanDto?> GetBeanByIdAsync(int id);
    Task<BeanDto> CreateBeanAsync(CreateBeanDto dto);
    Task<bool> UpdateBeanAsync(int id, UpdateBeanDto dto);
    Task<bool> DeleteBeanAsync(int id);
}