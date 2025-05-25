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

    public IEnumerable<BeanDto> GetAllBeans()
    {
        return _repository.GetAll().Select(MapToDto);
    }

    public BeanDto? GetBeanById(int id)
    {
        var bean = _repository.GetById(id);
        return bean != null ? MapToDto(bean) : null;
    }

    public BeanDto CreateBean(CreateBeanDto dto)
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

        var addedBean = _repository.Add(bean);
        return MapToDto(addedBean);
    }

    public bool UpdateBean(int id, UpdateBeanDto dto)
    {
        var bean = _repository.GetById(id);
        if (bean is null) return false;

        bean.Name = dto.Name;
        bean.OriginId = dto.OriginId;
        bean.RoastLevel = dto.RoastLevel;
        bean.Description = dto.Description;
        bean.Price = dto.Price;

        return _repository.Update(bean);
    }

    public bool DeleteBean(int id)
    {
        return _repository.Delete(id);
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