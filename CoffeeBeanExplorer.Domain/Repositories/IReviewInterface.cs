using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IReviewRepository
{
    IEnumerable<Review> GetAll();
    Review? GetById(int id);
    IEnumerable<Review> GetByBeanId(int beanId);
    IEnumerable<Review> GetByUserId(int userId);
    bool HasUserReviewedBean(int userId, int beanId);
    Review Add(Review review);
    bool Update(Review review);
    bool Delete(int id);
}