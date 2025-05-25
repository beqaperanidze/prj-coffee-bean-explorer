using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;

    public ReviewService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<ReviewDto> GetAllReviews()
    {
        return _repository.GetAll().Select(MapToDto);
    }

    public ReviewDto? GetReviewById(int id)
    {
        var review = _repository.GetById(id);
        return review != null ? MapToDto(review) : null;
    }

    public IEnumerable<ReviewDto> GetReviewsByBeanId(int beanId)
    {
        return _repository.GetByBeanId(beanId).Select(MapToDto);
    }

    public IEnumerable<ReviewDto> GetReviewsByUserId(int userId)
    {
        return _repository.GetByUserId(userId).Select(MapToDto);
    }

    public ReviewDto CreateReview(CreateReviewDto dto, int userId)
    {
        if (_repository.HasUserReviewedBean(userId, dto.BeanId))
        {
            throw new InvalidOperationException("User has already reviewed this bean");
        }

        var review = new Review
        {
            UserId = userId,
            BeanId = dto.BeanId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            User = new User { Id = userId },
            Bean = new Bean { Id = dto.BeanId }
        };

        var addedReview = _repository.Add(review);
        return MapToDto(addedReview);
    }

    public bool UpdateReview(int id, UpdateReviewDto dto, int userId)
    {
        var review = _repository.GetById(id);
        if (review is null) return false;
        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        return _repository.Update(review);
    }

    public bool DeleteReview(int id)
    {
        return _repository.Delete(id);
    }

    private static ReviewDto MapToDto(Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            Username = review.User?.Username ?? string.Empty,
            BeanId = review.BeanId,
            BeanName = review.Bean?.Name ?? string.Empty,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}