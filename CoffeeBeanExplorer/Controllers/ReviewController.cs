using System.Security.Claims;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
[Authorize]
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
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var review = await reviewService.GetReviewByIdAsync(parsedId);
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
    ///     Creates a new review for the authenticated user
    /// </summary>
    /// <param name="createDto">The review data to create</param>
    /// <returns>The created review with its new ID</returns>
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create(CreateReviewDto createDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var (review, errorMessage) = await reviewService.CreateReviewAsync(createDto, currentParsedId);

        if (errorMessage is not null)
            return BadRequest(new { Message = errorMessage });

        return CreatedAtAction(nameof(GetById), new { id = review!.Id }, review);
    }

    /// <summary>
    ///     Updates an existing review by ID
    /// </summary>
    /// <param name="id">ID of the review to update</param>
    /// <param name="updateDto">New review data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(string id, UpdateReviewDto updateDto)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid review ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdminOrBrewer = User.IsInRole("Admin") || User.IsInRole("Brewer");

        var review = await reviewService.GetReviewByIdAsync(parsedId);
        if (review == null)
            return NotFound();

        if (review.UserId != currentParsedId && !isAdminOrBrewer)
            return Forbid("You can only update your own reviews.");

        var success = await reviewService.UpdateReviewAsync(parsedId, updateDto, currentParsedId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Deletes a review by its ID
    /// </summary>
    /// <param name="id">ID of the review to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdminOrBrewer = User.IsInRole("Admin") || User.IsInRole("Brewer");

        var review = await reviewService.GetReviewByIdAsync(parsedId);
        if (review == null)
            return NotFound();

        if (review.UserId != currentParsedId && !isAdminOrBrewer)
            return Forbid("You can only delete your own reviews.");

        var success = await reviewService.DeleteReviewAsync(parsedId);
        if (!success) return NotFound();
        return NoContent();
    }
}
