using System;
using System.Collections.Generic;
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
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves a specific review by its ID
    /// </summary>
    /// <param name="id">The ID of the review to retrieve</param>
    /// <returns>The requested review or NotFound</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review is null) return NotFound();
        return Ok(review);
    }

    /// <summary>
    /// Retrieves all reviews for a specific bean
    /// </summary>
    /// <param name="beanId">The bean ID to get reviews for</param>
    /// <returns>List of reviews for the specified bean</returns>
    [HttpGet("beans/{beanId:int}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByBeanId(int beanId)
    {
        var reviews = await _reviewService.GetReviewsByBeanIdAsync(beanId);
        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves all reviews by a specific user
    /// </summary>
    /// <param name="userId">The user ID to get reviews for</param>
    /// <returns>List of reviews by the specified user</returns>
    [HttpGet("users/{userId:int}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByUserId(int userId)
    {
        var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
        return Ok(reviews);
    }

    /// <summary>
    /// Creates a new review
    /// </summary>
    /// <param name="createDto">The review data to create</param>
    /// <param name="userId">ID of the user creating the review</param>
    /// <returns>The created review with its new ID</returns>
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create(CreateReviewDto createDto, [FromQuery] int userId)
    {
        try
        {
            var review = await _reviewService.CreateReviewAsync(createDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
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
    public async Task<IActionResult> Update(int id, UpdateReviewDto updateDto, [FromQuery] int userId)
    {
        var success = await _reviewService.UpdateReviewAsync(id, updateDto, userId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a review by its ID
    /// </summary>
    /// <param name="id">ID of the review to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _reviewService.DeleteReviewAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
