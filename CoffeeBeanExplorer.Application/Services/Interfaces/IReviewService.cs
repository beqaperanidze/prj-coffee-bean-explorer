using CoffeeBeanExplorer.Application.DTOs;


namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
    Task<ReviewDto?> GetReviewByIdAsync(int id);
    Task<IEnumerable<ReviewDto>> GetReviewsByBeanIdAsync(int beanId);
    Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId);
    Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int userId);
    Task<bool> UpdateReviewAsync(int id, UpdateReviewDto dto, int userId);
    Task<bool> DeleteReviewAsync(int id);
}