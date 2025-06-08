using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class ReviewService(
    IReviewRepository repository,
    IBeanRepository beanRepository,
    IUserRepository userRepository,
    IMapper mapper)
    : IReviewService
{
    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto?> GetReviewByIdAsync(int id)
    {
        var review = await repository.GetByIdAsync(id);
        return review is not null ? mapper.Map<ReviewDto>(review) : null;
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByBeanIdAsync(int beanId)
    {
        if (!await beanRepository.ExistsAsync(beanId))
            return Enumerable.Empty<ReviewDto>();

        var reviews = await repository.GetByBeanIdAsync(beanId);
        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
    {
        if (!await userRepository.ExistsAsync(userId))
            return Enumerable.Empty<ReviewDto>();

        var reviews = await repository.GetByUserIdAsync(userId);
        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<(ReviewDto? Review, string? ErrorMessage)> CreateReviewAsync(CreateReviewDto dto, int userId)
    {
        if (!await userRepository.ExistsAsync(userId))
            return (null, $"User with ID {userId} does not exist");

        if (!await beanRepository.ExistsAsync(dto.BeanId))
            return (null, $"Bean with ID {dto.BeanId} does not exist");

        if (await repository.HasUserReviewedBeanAsync(userId, dto.BeanId))
            return (null, "User has already reviewed this bean");

        var review = mapper.Map<Review>(dto);
        review.UserId = userId;

        var addedReview = await repository.AddAsync(review);
        var fullReview = await repository.GetByIdAsync(addedReview.Id);

        return (mapper.Map<ReviewDto>(fullReview!), null);
    }

    public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto dto, int userId)
    {
        if (!await userRepository.ExistsAsync(userId))
            return false;

        var review = await repository.GetByIdAsync(id);
        if (review is null || review.UserId != userId)
            return false;

        mapper.Map(dto, review);
        return await repository.UpdateAsync(review);
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await repository.GetByIdAsync(id);
        if (review is null)
            return false;

        return await repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsAsync(int? beanId, int? userId)
    {
        if (beanId.HasValue && !await beanRepository.ExistsAsync(beanId.Value))
            return Enumerable.Empty<ReviewDto>();

        if (userId.HasValue && !await userRepository.ExistsAsync(userId.Value))
            return Enumerable.Empty<ReviewDto>();

        var reviews = (beanId, userId) switch
        {
            ({ } bid, { } uid) => (await repository.GetAllAsync())
                .Where(r => r.BeanId == bid && r.UserId == uid),
            ({ } bid, null) => await repository.GetByBeanIdAsync(bid),
            (null, { } uid) => await repository.GetByUserIdAsync(uid),
            (null, null) => await repository.GetAllAsync()
        };

        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }
}