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

    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _repository.GetAllAsync();
        return reviews.Select(MapToDto);
    }

    public async Task<ReviewDto?> GetReviewByIdAsync(int id)
    {
        var review = await _repository.GetByIdAsync(id);
        return review != null ? MapToDto(review) : null;
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByBeanIdAsync(int beanId)
    {
        var reviews = await _repository.GetByBeanIdAsync(beanId);
        return reviews.Select(MapToDto);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
    {
        var reviews = await _repository.GetByUserIdAsync(userId);
        return reviews.Select(MapToDto);
    }

    public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int userId)
    {
        if (await _repository.HasUserReviewedBeanAsync(userId, dto.BeanId))
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

        var addedReview = await _repository.AddAsync(review);
        return MapToDto(addedReview);
    }

    public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto dto, int userId)
    {
        var review = await _repository.GetByIdAsync(id);
        if (review is null || review.UserId != userId) return false;

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        return await _repository.UpdateAsync(review);
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        return await _repository.DeleteAsync(id);
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