﻿using System.Security.Claims;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
[Authorize]
public class ReviewController(IReviewService reviewService, ILogger<ReviewController> logger) : ControllerBase
{
    /// <summary>
    ///     Retrieves all reviews
    /// </summary>
    /// <returns>List of all reviews</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all reviews");
        var reviews = await reviewService.GetAllReviewsAsync();
        logger.LogInformation("Successfully retrieved {ReviewCount} reviews", reviews.Count());
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
        {
            logger.LogWarning("Invalid review ID format: {ReviewId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Retrieving review with ID: {ReviewId}", parsedId);
        var review = await reviewService.GetReviewByIdAsync(parsedId);
        if (review is null)
        {
            logger.LogWarning("Review not found for ID: {ReviewId}", parsedId);
            throw new NotFoundException($"Review with ID {parsedId} not found.");
        }

        logger.LogInformation("Review retrieved successfully with ID: {ReviewId}", parsedId);
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
        logger.LogInformation("Filtering reviews by beanId: {BeanId}, userId: {UserId}", beanId, userId);
        var reviews = await reviewService.GetReviewsAsync(beanId, userId);
        logger.LogInformation("Successfully retrieved {ReviewCount} filtered reviews", reviews.Count());
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
        {
            logger.LogWarning("User authentication failed or invalid ID when creating review");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        logger.LogInformation("Creating new review for bean ID {BeanId} by user ID {UserId}",
            createDto.BeanId, currentParsedId);

        var (review, errorMessage) = await reviewService.CreateReviewAsync(createDto, currentParsedId);

        if (errorMessage is not null)
        {
            logger.LogWarning("Failed to create review: {ErrorMessage}", errorMessage);
            throw new BadRequestException(errorMessage);
        }

        logger.LogInformation("Review created successfully with ID: {ReviewId}", review!.Id);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
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
        {
            logger.LogWarning("Invalid review ID format for update: {ReviewId}", id);
            throw new BadRequestException("Invalid review ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID when updating review");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        var isAdminOrBrewer = User.IsInRole("Admin") || User.IsInRole("Brewer");

        logger.LogInformation("Retrieving review with ID: {ReviewId} for update", parsedId);
        var review = await reviewService.GetReviewByIdAsync(parsedId);
        if (review == null)
        {
            logger.LogWarning("Review not found for update, ID: {ReviewId}", parsedId);
            throw new NotFoundException($"Review with ID {parsedId} not found.");
        }

        if (review.UserId != currentParsedId && !isAdminOrBrewer)
        {
            logger.LogWarning("Unauthorized update attempt by user: {CurrentUserId} for review ID: {ReviewId}",
                currentParsedId, parsedId);
            throw new UnauthorizedException("You can only update your own reviews.");
        }

        logger.LogInformation("Updating review with ID: {ReviewId}", parsedId);

        var userIdForUpdate = isAdminOrBrewer ? review.UserId : currentParsedId;

        var success = await reviewService.UpdateReviewAsync(parsedId, updateDto, userIdForUpdate);
        if (!success)
        {
            logger.LogWarning("Failed to update review with ID: {ReviewId}", parsedId);
            throw new NotFoundException($"Review with ID {parsedId} not found.");
        }

        logger.LogInformation("Review {ReviewId} updated successfully", parsedId);
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
        {
            logger.LogWarning("Invalid review ID format for deletion: {ReviewId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID when deleting review");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        var isAdminOrBrewer = User.IsInRole("Admin") || User.IsInRole("Brewer");

        logger.LogInformation("Retrieving review with ID: {ReviewId} for deletion", parsedId);
        var review = await reviewService.GetReviewByIdAsync(parsedId);
        if (review == null)
        {
            logger.LogWarning("Review not found for deletion, ID: {ReviewId}", parsedId);
            throw new NotFoundException($"Review with ID {parsedId} not found.");
        }

        if (review.UserId != currentParsedId && !isAdminOrBrewer)
        {
            logger.LogWarning("Unauthorized deletion attempt by user: {CurrentUserId} for review ID: {ReviewId}",
                currentParsedId, parsedId);
            throw new UnauthorizedException("You can only delete your own reviews.");
        }

        logger.LogInformation("Deleting review with ID: {ReviewId}", parsedId);
        var success = await reviewService.DeleteReviewAsync(parsedId);
        if (!success)
        {
            logger.LogWarning("Failed to delete review with ID: {ReviewId}", parsedId);
            throw new NotFoundException($"Review with ID {parsedId} not found.");
        }

        logger.LogInformation("Review {ReviewId} deleted successfully", parsedId);
        return NoContent();
    }
}
