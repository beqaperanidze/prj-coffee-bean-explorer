using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface ITagRepository
{
    IEnumerable<Tag> GetAll();
    Tag? GetById(int id);
    IEnumerable<Tag> GetByBeanId(int beanId);
    Tag Add(Tag tag);
    bool Update(Tag tag);
    bool Delete(int id);
    bool AddTagToBean(int tagId, int beanId);
    bool RemoveTagFromBean(int tagId, int beanId);
}