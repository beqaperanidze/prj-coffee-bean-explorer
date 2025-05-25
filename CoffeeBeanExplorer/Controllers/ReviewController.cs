using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Retrieves all reviews
    /// </summary>
    /// <returns>List of all reviews</returns>
    [HttpGet]
    public ActionResult<IEnumerable<ReviewDto>> GetAll()
    {
        var reviews = _reviewService.GetAllReviews();
        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves a specific review by its ID
    /// </summary>
    /// <param name="id">The ID of the review to retrieve</param>
    /// <returns>The requested review or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<ReviewDto> GetById(int id)
    {
        var review = _reviewService.GetReviewById(id);
        if (review is null) return NotFound();
        return Ok(review);
    }

    /// <summary>
    /// Retrieves all reviews for a specific bean
    /// </summary>
    /// <param name="beanId">The bean ID to get reviews for</param>
    /// <returns>List of reviews for the specified bean</returns>
    [HttpGet("byBean/{beanId:int}")]
    public ActionResult<IEnumerable<ReviewDto>> GetByBeanId(int beanId)
    {
        var reviews = _reviewService.GetReviewsByBeanId(beanId);
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
        var reviews = _reviewService.GetReviewsByUserId(userId);
        return Ok(reviews);
    }

    /// <summary>
    /// Creates a new review
    /// </summary>
    /// <param name="createDto">The review data to create</param>
    /// <param name="userId">ID of the user creating the review</param>
    /// <returns>The created review with its new ID</returns>
    [HttpPost]
    public ActionResult<ReviewDto> Create(CreateReviewDto createDto, [FromQuery] int userId)
    {
        try
        {
            var review = _reviewService.CreateReview(createDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
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
        var success = _reviewService.UpdateReview(id, updateDto, userId);
        if (!success) return NotFound();
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
        var success = _reviewService.DeleteReview(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
