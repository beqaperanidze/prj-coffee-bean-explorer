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

    public IEnumerable<TagDto> GetAllTags()
    {
        return _repository.GetAll().Select(MapToDto);
    }

    public TagDto? GetTagById(int id)
    {
        var tag = _repository.GetById(id);
        return tag != null ? MapToDto(tag) : null;
    }

    public IEnumerable<TagDto> GetTagsByBeanId(int beanId)
    {
        return _repository.GetByBeanId(beanId).Select(MapToDto);
    }

    public TagDto CreateTag(CreateTagDto dto)
    {
        var tag = new Tag
        {
            Name = dto.Name
        };

        var addedTag = _repository.Add(tag);
        return MapToDto(addedTag);
    }

    public bool UpdateTag(int id, UpdateTagDto dto)
    {
        var tag = _repository.GetById(id);
        if (tag is null) return false;

        tag.Name = dto.Name;

        return _repository.Update(tag);
    }

    public bool DeleteTag(int id)
    {
        return _repository.Delete(id);
    }

    public bool AddTagToBean(int tagId, int beanId)
    {
        return _repository.AddTagToBean(tagId, beanId);
    }

    public bool RemoveTagFromBean(int tagId, int beanId)
    {
        return _repository.RemoveTagFromBean(tagId, beanId);
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