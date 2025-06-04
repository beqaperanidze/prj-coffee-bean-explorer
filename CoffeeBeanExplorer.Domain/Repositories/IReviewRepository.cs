using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Domain.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(int id);
    Task<IEnumerable<Review>> GetByBeanIdAsync(int beanId);
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
    Task<bool> HasUserReviewedBeanAsync(int userId, int beanId);
    Task<Review> AddAsync(Review review);
    Task<bool> UpdateAsync(Review review);
    Task<bool> DeleteAsync(int id);
}