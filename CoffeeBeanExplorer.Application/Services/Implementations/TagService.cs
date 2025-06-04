using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class TagService(ITagRepository repository) : ITagService
{
    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await repository.GetAllAsync();
        return tags.Select(MapToDto);
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var tag = await repository.GetByIdAsync(id);
        return tag != null ? MapToDto(tag) : null;
    }

    public async Task<IEnumerable<TagDto>> GetTagsByBeanIdAsync(int beanId)
    {
        var tags = await repository.GetByBeanIdAsync(beanId);
        return tags.Select(MapToDto);
    }

    public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
    {
        var tag = new Tag
        {
            Name = dto.Name
        };

        var addedTag = await repository.AddAsync(tag);
        return MapToDto(addedTag);
    }

    public async Task<bool> UpdateTagAsync(int id, UpdateTagDto dto)
    {
        var tag = await repository.GetByIdAsync(id);
        if (tag is null) return false;

        tag.Name = dto.Name;

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
        return beans.Select(MapBeanToDto);
    }

    private static BeanDto MapBeanToDto(Bean bean)
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

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }
}