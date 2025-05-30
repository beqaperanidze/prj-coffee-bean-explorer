using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class BeanService(IBeanRepository repository) : IBeanService
{
    public async Task<IEnumerable<BeanDto>> GetAllBeansAsync()
    {
        var beans = await repository.GetAllAsync();
        return beans.Select(MapToDto);
    }

    public async Task<BeanDto?> GetBeanByIdAsync(int id)
    {
        var bean = await repository.GetByIdAsync(id);
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
            Price = dto.Price
        };

        var addedBean = await repository.AddAsync(bean);

        var fullBean = await repository.GetByIdAsync(addedBean.Id);

        return MapToDto(fullBean!);
    }

    public async Task<bool> UpdateBeanAsync(int id, UpdateBeanDto dto)
    {
        var bean = await repository.GetByIdAsync(id);
        if (bean is null) return false;

        bean.Name = dto.Name;
        bean.OriginId = dto.OriginId;
        bean.RoastLevel = dto.RoastLevel;
        bean.Description = dto.Description;
        bean.Price = dto.Price;

        return await repository.UpdateAsync(bean);
    }

    public async Task<bool> DeleteBeanAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    private static BeanDto MapToDto(Bean bean)
    {
        return new BeanDto
        {
            Id = bean.Id,
            Name = bean.Name,
            OriginId = bean.OriginId,
            OriginCountry = bean.Origin?.Country ?? string.Empty,
            OriginRegion = bean.Origin?.Region,
            RoastLevel = bean.RoastLevel,
            Description = bean.Description,
            Price = bean.Price,
            Tags = bean.BeanTags?.Select(bt => new TagDto
            {
                Id = bt.Tag!.Id,
                Name = bt.Tag.Name
            }).ToList() ?? []
        };
    }
}