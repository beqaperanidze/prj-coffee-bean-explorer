using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class BeanService(IBeanRepository repository, IMapper mapper) : IBeanService
{
    public async Task<IEnumerable<BeanDto>> GetAllBeansAsync()
    {
        var beans = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<BeanDto>>(beans);
    }

    public async Task<BeanDto?> GetBeanByIdAsync(int id)
    {
        var bean = await repository.GetByIdAsync(id);
        return bean is not null ? mapper.Map<BeanDto>(bean) : null;
    }

    public async Task<BeanDto> CreateBeanAsync(CreateBeanDto dto)
    {
        var bean = mapper.Map<Bean>(dto);
        var addedBean = await repository.AddAsync(bean);
        var fullBean = await repository.GetByIdAsync(addedBean.Id);
        return mapper.Map<BeanDto>(fullBean!);
    }

    public async Task<bool> UpdateBeanAsync(int id, UpdateBeanDto dto)
    {
        var bean = await repository.GetByIdAsync(id);
        if (bean is null) return false;

        mapper.Map(dto, bean);
        return await repository.UpdateAsync(bean);
    }

    public async Task<bool> DeleteBeanAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }
}