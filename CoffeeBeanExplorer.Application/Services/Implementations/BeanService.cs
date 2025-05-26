using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;


namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class BeanService : IBeanService
{
    private readonly IBeanRepository _repository;

    public BeanService(IBeanRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BeanDto>> GetAllBeansAsync()
    {
        var beans = await _repository.GetAllAsync();
        return beans.Select(MapToDto);
    }

    public async Task<BeanDto?> GetBeanByIdAsync(int id)
    {
        var bean = await _repository.GetByIdAsync(id);
        return bean != null ? MapToDto(bean) : null;
    }

    public async Task<BeanDto> CreateBeanAsync(CreateBeanDto dto)
    {
        var bean = new Bean
        {
            Name = dto.Name,
            OriginId = dto.OriginId,
            RoastLevel = dto.RoastLevel,
            Description = dto.Description,
            Price = dto.Price,
            Origin = new Origin()
        };

        var addedBean = await _repository.AddAsync(bean);
        return MapToDto(addedBean);
    }

    public async Task<bool> UpdateBeanAsync(int id, UpdateBeanDto dto)
    {
        var bean = await _repository.GetByIdAsync(id);
        if (bean is null) return false;

        bean.Name = dto.Name;
        bean.OriginId = dto.OriginId;
        bean.RoastLevel = dto.RoastLevel;
        bean.Description = dto.Description;
        bean.Price = dto.Price;

        return await _repository.UpdateAsync(bean);
    }

    public async Task<bool> DeleteBeanAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static BeanDto MapToDto(Bean bean) => new BeanDto
    {
        Id = bean.Id,
        Name = bean.Name,
        OriginId = bean.OriginId,
        OriginCountry = bean.Origin?.Country ?? string.Empty,
        OriginRegion = bean.Origin?.Region,
        RoastLevel = bean.RoastLevel,
        Description = bean.Description,
        Price = bean.Price,
        CreatedAt = bean.CreatedAt,
        UpdatedAt = bean.UpdatedAt
    };
}