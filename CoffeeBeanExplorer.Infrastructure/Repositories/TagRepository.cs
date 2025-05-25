using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private static readonly List<Tag> _tags = [];
    private static readonly List<BeanTag> _beanTags = [];
    private static int _nextId = 1;

    public IEnumerable<Tag> GetAll() => _tags;

    public Tag? GetById(int id)
    {
        return _tags.FirstOrDefault(t => t.Id == id);
    }

    public IEnumerable<Tag> GetByBeanId(int beanId)
    {
        var tagIds = _beanTags
            .Where(bt => bt.BeanId == beanId)
            .Select(bt => bt.TagId);

        return _tags.Where(t => tagIds.Contains(t.Id));
    }

    public Tag Add(Tag tag)
    {
        tag.Id = _nextId++;
        tag.CreatedAt = DateTime.UtcNow;
        tag.UpdatedAt = DateTime.UtcNow;
        _tags.Add(tag);
        return tag;
    }

    public bool Update(Tag tag)
    {
        var existingTag = _tags.FirstOrDefault(t => t.Id == tag.Id);
        if (existingTag is null) return false;

        existingTag.Name = tag.Name;
        existingTag.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public bool Delete(int id)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == id);
        if (tag is null) return false;

        _beanTags.RemoveAll(bt => bt.TagId == id);

        return _tags.Remove(tag);
    }

    public bool AddTagToBean(int tagId, int beanId)
    {
        if (_beanTags.Any(bt => bt.TagId == tagId && bt.BeanId == beanId))
        {
            return false; 
        }

        var tag = GetById(tagId);
        var bean = new Bean { Id = beanId }; 

        if (tag == null) return false;

        _beanTags.Add(new BeanTag 
        { 
            BeanId = beanId, 
            TagId = tagId,
            CreatedAt = DateTime.UtcNow,
            Bean = bean,
            Tag = tag
        });
        
        return true;
    }

    public bool RemoveTagFromBean(int tagId, int beanId)
    {
        var beanTag = _beanTags.FirstOrDefault(bt => bt.TagId == tagId && bt.BeanId == beanId);
        return beanTag is not null && _beanTags.Remove(beanTag);
    }
}