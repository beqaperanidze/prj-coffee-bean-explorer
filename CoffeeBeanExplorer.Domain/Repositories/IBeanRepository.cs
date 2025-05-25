using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IBeanRepository
{
    IEnumerable<Bean> GetAll();
    Bean? GetById(int id);
    Bean Add(Bean bean);
    bool Update(Bean bean);
    bool Delete(int id);
}