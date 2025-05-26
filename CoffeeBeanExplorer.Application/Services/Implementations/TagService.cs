using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;


namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _repository.GetAllAsync();
        return tags.Select(MapToDto);
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var tag = await _repository.GetByIdAsync(id);
        return tag != null ? MapToDto(tag) : null;
    }

    public async Task<IEnumerable<TagDto>> GetTagsByBeanIdAsync(int beanId)
    {
        var tags = await _repository.GetByBeanIdAsync(beanId);
        return tags.Select(MapToDto);
    }

    public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
    {
        var tag = new Tag
        {
            Name = dto.Name
        };

        var addedTag = await _repository.AddAsync(tag);
        return MapToDto(addedTag);
    }

    public async Task<bool> UpdateTagAsync(int id, UpdateTagDto dto)
    {
        var tag = await _repository.GetByIdAsync(id);
        if (tag is null) return false;

        tag.Name = dto.Name;

        return await _repository.UpdateAsync(tag);
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<bool> AddTagToBeanAsync(int tagId, int beanId)
    {
        return await _repository.AddTagToBeanAsync(tagId, beanId);
    }

    public async Task<bool> RemoveTagFromBeanAsync(int tagId, int beanId)
    {
        return await _repository.RemoveTagFromBeanAsync(tagId, beanId);
    }

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        };
    }
}