using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllTagsAsync();
    Task<TagDto?> GetTagByIdAsync(int id);
    Task<IEnumerable<TagDto>> GetTagsByBeanIdAsync(int beanId);
    Task<TagDto> CreateTagAsync(CreateTagDto dto);
    Task<bool> UpdateTagAsync(int id, UpdateTagDto dto);
    Task<bool> DeleteTagAsync(int id);
    Task<bool> AddTagToBeanAsync(int tagId, int beanId);
    Task<bool> RemoveTagFromBeanAsync(int tagId, int beanId);
}