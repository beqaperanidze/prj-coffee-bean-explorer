using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    /// <summary>
    ///     Retrieves all reviews
    /// </summary>
    /// <returns>List of all reviews</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll()
    {
        var reviews = await reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    /// <summary>
    ///     Retrieves a specific review by its ID
    /// </summary>
    /// <param name="id">The ID of the review to retrieve</param>
    /// <returns>The requested review or NotFound</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id)
    {
        var review = await reviewService.GetReviewByIdAsync(id);
        if (review is null) return NotFound();
        return Ok(review);
    }

    /// <summary>
    ///     Retrieves reviews filtered by bean and/or user
    /// </summary>
    /// <param name="beanId">The bean ID to filter reviews (optional)</param>
    /// <param name="userId">The user ID to filter reviews (optional)</param>
    /// <returns>List of filtered reviews</returns>
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetFiltered([FromQuery] int? beanId,
        [FromQuery] int? userId)
    {
        var reviews = await reviewService.GetReviewsAsync(beanId, userId);
        return Ok(reviews);
    }

    /// <summary>
    ///     Creates a new review
    /// </summary>
    /// <param name="createDto">The review data to create</param>
    /// <param name="userId">ID of the user creating the review</param>
    /// <returns>The created review with its new ID</returns>
    [HttpPost("users/{userId:int}")]
    public async Task<ActionResult<ReviewDto>> Create(CreateReviewDto createDto, int userId)
    {
        var (review, errorMessage) = await reviewService.CreateReviewAsync(createDto, userId);

        if (errorMessage != null)
            return BadRequest(new { Message = errorMessage });

        return CreatedAtAction(nameof(GetById), new { id = review!.Id }, review);
    }

    /// <summary>
    ///     Updates an existing review by ID
    /// </summary>
    /// <param name="id">ID of the review to update</param>
    /// <param name="updateDto">New review data</param>
    /// <param name="userId">ID of the user updating the review</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}/users/{userId:int}")]
    public async Task<IActionResult> Update(int id, UpdateReviewDto updateDto, int userId)
    {
        var success = await reviewService.UpdateReviewAsync(id, updateDto, userId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Deletes a review by its ID
    /// </summary>
    /// <param name="id">ID of the review to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await reviewService.DeleteReviewAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
