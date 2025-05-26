using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;


namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private static readonly List<Tag> _tags = [];
    private static readonly List<BeanTag> _beanTags = [];
    private static int _nextId = 1;

    public Task<IEnumerable<Tag>> GetAllAsync() => Task.FromResult<IEnumerable<Tag>>(_tags);

    public Task<Tag?> GetByIdAsync(int id)
    {
        return Task.FromResult(_tags.FirstOrDefault(t => t.Id == id));
    }

    public Task<IEnumerable<Tag>> GetByBeanIdAsync(int beanId)
    {
        var tagIds = _beanTags
            .Where(bt => bt.BeanId == beanId)
            .Select(bt => bt.TagId);

        return Task.FromResult(_tags.Where(t => tagIds.Contains(t.Id)));
    }

    public Task<Tag> AddAsync(Tag tag)
    {
        tag.Id = _nextId++;
        tag.CreatedAt = DateTime.UtcNow;
        tag.UpdatedAt = DateTime.UtcNow;
        _tags.Add(tag);
        return Task.FromResult(tag);
    }

    public Task<bool> UpdateAsync(Tag tag)
    {
        var existingTag = _tags.FirstOrDefault(t => t.Id == tag.Id);
        if (existingTag is null) return Task.FromResult(false);

        existingTag.Name = tag.Name;
        existingTag.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == id);
        if (tag is null) return Task.FromResult(false);

        _beanTags.RemoveAll(bt => bt.TagId == id);

        return Task.FromResult(_tags.Remove(tag));
    }

    public Task<bool> AddTagToBeanAsync(int tagId, int beanId)
    {
        if (_beanTags.Any(bt => bt.TagId == tagId && bt.BeanId == beanId))
        {
            return Task.FromResult(false);
        }

        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        var bean = new Bean { Id = beanId };

        if (tag == null) return Task.FromResult(false);

        _beanTags.Add(new BeanTag
        {
            BeanId = beanId,
            TagId = tagId,
            CreatedAt = DateTime.UtcNow,
            Bean = bean,
            Tag = tag
        });

        return Task.FromResult(true);
    }

    public Task<bool> RemoveTagFromBeanAsync(int tagId, int beanId)
    {
        var beanTag = _beanTags.FirstOrDefault(bt => bt.TagId == tagId && bt.BeanId == beanId);
        return Task.FromResult(beanTag is not null && _beanTags.Remove(beanTag));
    }
}