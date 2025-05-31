using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class ReviewService(IReviewRepository repository, IMapper mapper) : IReviewService
{
    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto?> GetReviewByIdAsync(int id)
    {
        var review = await repository.GetByIdAsync(id);
        return review != null ? mapper.Map<ReviewDto>(review) : null;
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByBeanIdAsync(int beanId)
    {
        var reviews = await repository.GetByBeanIdAsync(beanId);
        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
    {
        var reviews = await repository.GetByUserIdAsync(userId);
        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<(ReviewDto? Review, string? ErrorMessage)> CreateReviewAsync(CreateReviewDto dto, int userId)
    {
        if (await repository.HasUserReviewedBeanAsync(userId, dto.BeanId))
            return (null, "User has already reviewed this bean");

        var review = new Review
        {
            UserId = userId,
            BeanId = dto.BeanId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        var addedReview = await repository.AddAsync(review);
        var fullReview = await repository.GetByIdAsync(addedReview.Id);

        return (mapper.Map<ReviewDto>(fullReview!), null);
    }

    public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto dto, int userId)
    {
        var review = await repository.GetByIdAsync(id);
        if (review is null || review.UserId != userId) return false;

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        return await repository.UpdateAsync(review);
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsAsync(int? beanId, int? userId)
    {
        IEnumerable<Review> reviews;

        if (beanId.HasValue && userId.HasValue)
        {
            var allReviews = await repository.GetAllAsync();
            reviews = allReviews.Where(r => r.BeanId == beanId.Value && r.UserId == userId.Value);
        }
        else if (beanId.HasValue)
        {
            reviews = await repository.GetByBeanIdAsync(beanId.Value);
        }
        else if (userId.HasValue)
        {
            reviews = await repository.GetByUserIdAsync(userId.Value);
        }
        else
        {
            reviews = await repository.GetAllAsync();
        }

        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }
}