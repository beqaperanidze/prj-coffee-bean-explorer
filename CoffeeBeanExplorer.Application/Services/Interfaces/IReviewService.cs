using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IReviewService
{
    IEnumerable<ReviewDto> GetAllReviews();
    ReviewDto? GetReviewById(int id);
    IEnumerable<ReviewDto> GetReviewsByBeanId(int beanId);
    IEnumerable<ReviewDto> GetReviewsByUserId(int userId);
    ReviewDto CreateReview(CreateReviewDto dto, int userId);
    bool UpdateReview(int id, UpdateReviewDto dto, int userId);
    bool DeleteReview(int id);
}