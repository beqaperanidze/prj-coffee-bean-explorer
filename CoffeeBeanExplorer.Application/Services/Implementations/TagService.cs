using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class TagService(ITagRepository repository, IMapper mapper) : ITagService
{
    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var tag = await repository.GetByIdAsync(id);
        return tag is not null ? mapper.Map<TagDto>(tag) : null;
    }

    public async Task<IEnumerable<TagDto>> GetTagsByBeanIdAsync(int beanId)
    {
        var tags = await repository.GetByBeanIdAsync(beanId);
        return mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
    {
        var tag = mapper.Map<Tag>(dto);
        var addedTag = await repository.AddAsync(tag);
        return mapper.Map<TagDto>(addedTag);
    }

    public async Task<bool> UpdateTagAsync(int id, UpdateTagDto dto)
    {
        var tag = await repository.GetByIdAsync(id);
        if (tag is null) return false;

        mapper.Map(dto, tag);
        return await repository.UpdateAsync(tag);
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    public async Task<bool> AddTagToBeanAsync(int tagId, int beanId)
    {
        return await repository.AddTagToBeanAsync(tagId, beanId);
    }

    public async Task<bool> RemoveTagFromBeanAsync(int tagId, int beanId)
    {
        return await repository.RemoveTagFromBeanAsync(tagId, beanId);
    }

    public async Task<IEnumerable<BeanDto>> GetBeansByTagIdAsync(int tagId)
    {
        var beans = await repository.GetBeansByTagIdAsync(tagId);
        return mapper.Map<IEnumerable<BeanDto>>(beans);
    }
}