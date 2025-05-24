using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Models;
using CoffeeBeanExplorer.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private static readonly List<Review> Reviews = [];
    private static int _nextId = 1;

    /// <summary>
    /// Retrieves all reviews
    /// </summary>
    /// <returns>List of all reviews</returns>
    [HttpGet]
    public ActionResult<IEnumerable<ReviewDto>> GetAll()
    {
        var reviewDtos = Reviews.Select(MapToDto).ToList();
        return Ok(reviewDtos);
    }

    /// <summary>
    /// Retrieves a specific review by its ID
    /// </summary>
    /// <param name="id">The ID of the review to retrieve</param>
    /// <returns>The requested review or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<ReviewDto> GetById(int id)
    {
        var review = Reviews.FirstOrDefault(r => r.Id == id);
        if (review == null) return NotFound();
        return Ok(MapToDto(review));
    }

    /// <summary>
    /// Retrieves all reviews for a specific bean
    /// </summary>
    /// <param name="beanId">The bean ID to get reviews for</param>
    /// <returns>List of reviews for the specified bean</returns>
    [HttpGet("byBean/{beanId:int}")]
    public ActionResult<IEnumerable<ReviewDto>> GetByBeanId(int beanId)
    {
        var reviews = Reviews.Where(r => r.BeanId == beanId).Select(MapToDto).ToList();
        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves all reviews by a specific user
    /// </summary>
    /// <param name="userId">The user ID to get reviews for</param>
    /// <returns>List of reviews by the specified user</returns>
    [HttpGet("byUser/{userId:int}")]
    public ActionResult<IEnumerable<ReviewDto>> GetByUserId(int userId)
    {
        var reviews = Reviews.Where(r => r.UserId == userId).Select(MapToDto).ToList();
        return Ok(reviews);
    }

    /// <summary>
    /// Creates a new review
    /// </summary>
    /// <param name="createDto">The review data to create</param>
    /// <param name="userId">ID of the user creating the review (from auth context in a real app)</param>
    /// <returns>The created review with its new ID</returns>
    [HttpPost]
    public ActionResult<ReviewDto> Create(CreateReviewDto createDto, [FromQuery] int userId)
    {
        if (Reviews.Any(r => r.UserId == userId && r.BeanId == createDto.BeanId))
        {
            return BadRequest(new { Message = "User has already reviewed this bean" });
        }

        var review = new Review
        {
            Id = _nextId++,
            UserId = userId,
            BeanId = createDto.BeanId,
            Rating = createDto.Rating,
            Comment = createDto.Comment,
            User = new User { Id = userId },
            Bean = new Bean { Id = createDto.BeanId }
        };

        Reviews.Add(review);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, MapToDto(review));
    }

    /// <summary>
    /// Updates an existing review by ID
    /// </summary>
    /// <param name="id">ID of the review to update</param>
    /// <param name="updateDto">New review data</param>
    /// <param name="userId">ID of the user updating the review</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateReviewDto updateDto, [FromQuery] int userId)
    {
        var review = Reviews.FirstOrDefault(r => r.Id == id);
        if (review == null) return NotFound();

        review.Rating = updateDto.Rating;
        review.Comment = updateDto.Comment;
        review.UpdatedAt = DateTime.UtcNow;

        return NoContent();
    }

    /// <summary>
    /// Deletes a review by its ID
    /// </summary>
    /// <param name="id">ID of the review to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var review = Reviews.FirstOrDefault(r => r.Id == id);
        if (review == null) return NotFound();

        Reviews.Remove(review);
        return NoContent();
    }

    private static ReviewDto MapToDto(Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            Username = review.User.Username,
            BeanId = review.BeanId,
            BeanName = review.Bean.Name,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}
