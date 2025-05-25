using System.Collections.Generic;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IOriginRepository
{
    IEnumerable<Origin> GetAll();
    Origin? GetById(int id);
    Origin Add(Origin origin);
    bool Update(Origin origin);
    bool Delete(int id);
}