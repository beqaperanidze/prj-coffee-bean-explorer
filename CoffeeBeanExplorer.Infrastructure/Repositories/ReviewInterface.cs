using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private static readonly List<Review> _reviews = [];
    private static int _nextId = 1;

    public IEnumerable<Review> GetAll() => _reviews;

    public Review? GetById(int id)
    {
        return _reviews.FirstOrDefault(r => r.Id == id);
    }

    public IEnumerable<Review> GetByBeanId(int beanId)
    {
        return _reviews.Where(r => r.BeanId == beanId);
    }

    public IEnumerable<Review> GetByUserId(int userId)
    {
        return _reviews.Where(r => r.UserId == userId);
    }

    public bool HasUserReviewedBean(int userId, int beanId)
    {
        return _reviews.Any(r => r.UserId == userId && r.BeanId == beanId);
    }

    public Review Add(Review review)
    {
        review.Id = _nextId++;
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;
        _reviews.Add(review);
        return review;
    }

    public bool Update(Review review)
    {
        var existingReview = _reviews.FirstOrDefault(r => r.Id == review.Id);
        if (existingReview is null) return false;

        existingReview.Rating = review.Rating;
        existingReview.Comment = review.Comment;
        existingReview.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public bool Delete(int id)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == id);
        return review is not null && _reviews.Remove(review);
    }
}