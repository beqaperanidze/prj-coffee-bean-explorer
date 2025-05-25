using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface ITagService
{
    IEnumerable<TagDto> GetAllTags();
    TagDto? GetTagById(int id);
    IEnumerable<TagDto> GetTagsByBeanId(int beanId);
    TagDto CreateTag(CreateTagDto dto);
    bool UpdateTag(int id, UpdateTagDto dto);
    bool DeleteTag(int id);
    bool AddTagToBean(int tagId, int beanId);
    bool RemoveTagFromBean(int tagId, int beanId);
}