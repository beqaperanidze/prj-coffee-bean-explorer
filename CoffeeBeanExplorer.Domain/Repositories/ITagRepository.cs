using System.Collections.Generic;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<IEnumerable<Tag>> GetByBeanIdAsync(int beanId);
    Task<Tag> AddAsync(Tag tag);
    Task<bool> UpdateAsync(Tag tag);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddTagToBeanAsync(int tagId, int beanId);
    Task<bool> RemoveTagFromBeanAsync(int tagId, int beanId);
}