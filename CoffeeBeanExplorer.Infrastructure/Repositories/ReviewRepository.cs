using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;


namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private static readonly List<Review> _reviews = [];
    private static int _nextId = 1;

    public Task<IEnumerable<Review>> GetAllAsync() => Task.FromResult<IEnumerable<Review>>(_reviews);

    public Task<Review?> GetByIdAsync(int id)
    {
        return Task.FromResult(_reviews.FirstOrDefault(r => r.Id == id));
    }

    public Task<IEnumerable<Review>> GetByBeanIdAsync(int beanId)
    {
        return Task.FromResult(_reviews.Where(r => r.BeanId == beanId));
    }

    public Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
    {
        return Task.FromResult(_reviews.Where(r => r.UserId == userId));
    }

    public Task<bool> HasUserReviewedBeanAsync(int userId, int beanId)
    {
        return Task.FromResult(_reviews.Any(r => r.UserId == userId && r.BeanId == beanId));
    }

    public Task<Review> AddAsync(Review review)
    {
        review.Id = _nextId++;
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;
        _reviews.Add(review);
        return Task.FromResult(review);
    }

    public Task<bool> UpdateAsync(Review review)
    {
        var existingReview = _reviews.FirstOrDefault(r => r.Id == review.Id);
        if (existingReview is null) return Task.FromResult(false);

        existingReview.Rating = review.Rating;
        existingReview.Comment = review.Comment;
        existingReview.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(review is not null && _reviews.Remove(review));
    }
}