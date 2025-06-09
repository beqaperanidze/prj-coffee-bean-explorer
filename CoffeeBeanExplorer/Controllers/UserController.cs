using System.Security.Claims;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    /// <summary>
    ///     Retrieves the currently authenticated user
    /// </summary>
    /// <returns>The current user's information</returns>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var parsedId))
        {
            logger.LogWarning("Unauthorized access attempt with invalid user ID: {UserId}", userId);
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        logger.LogInformation("Retrieving current user information for ID: {UserId}", parsedId);
        var user = await userService.GetUserByIdAsync(parsedId);
        if (user == null)
        {
            logger.LogWarning("User not found for ID: {UserId}", parsedId);
            throw new NotFoundException($"User with ID {parsedId} not found.");
        }

        logger.LogInformation("User information retrieved successfully for ID: {UserId}", parsedId);
        return Ok(user);
    }

    /// <summary>
    ///     Retrieves all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all users");
        var users = await userService.GetAllUsersAsync();
        logger.LogInformation("Successfully retrieved {UserCount} users", users.Count());
        return Ok(users);
    }

    /// <summary>
    ///     Retrieves a specific user by ID
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The requested user or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid user ID format: {UserId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Retrieving user with ID: {UserId}", parsedId);
        var user = await userService.GetUserByIdAsync(parsedId);
        if (user == null)
        {
            logger.LogWarning("User not found for ID: {UserId}", parsedId);
            throw new NotFoundException($"User with ID {parsedId} not found.");
        }

        logger.LogInformation("User retrieved successfully with ID: {UserId}", parsedId);
        return Ok(user);
    }

    /// <summary>
    ///     Updates an existing user by ID
    /// </summary>
    /// <param name="id">ID of the user to update</param>
    /// <param name="userUpdateDto">New user data</param>
    /// <returns>The updated user data</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(string id, UserUpdateDto userUpdateDto)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid user ID format for update: {UserId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID: {CurrentUserId}", currentUserId);
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        var isAdmin = User.IsInRole("Admin");
        if (parsedId != currentParsedId && !isAdmin)
        {
            logger.LogWarning("Unauthorized update attempt by user: {CurrentUserId} for user ID: {UserId}",
                currentParsedId, parsedId);
            throw new UnauthorizedException("You can only update your own user information.");
        }

        try
        {
            logger.LogInformation("Updating user with ID: {UserId}", parsedId);
            var user = await userService.UpdateUserAsync(parsedId, userUpdateDto);
            if (user == null)
            {
                logger.LogWarning("User not found for update, ID: {UserId}", parsedId);
                throw new NotFoundException($"User with ID {parsedId} not found.");
            }

            logger.LogInformation("User {UserId} updated successfully", parsedId);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", parsedId);
            throw new InternalServerErrorException($"An error occurred while updating user: {ex.Message}");
        }
    }

    /// <summary>
    ///     Deletes a user by ID
    /// </summary>
    /// <param name="id">ID of the user to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid user ID format for deletion: {UserId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID: {CurrentUserId}", currentUserId);
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        var isAdmin = User.IsInRole("Admin");
        if (parsedId != currentParsedId && !isAdmin)
        {
            logger.LogWarning("Unauthorized deletion attempt by user: {CurrentUserId} for user ID: {UserId}",
                currentParsedId, parsedId);
            throw new UnauthorizedException("You can only delete your own user account.");
        }

        var success = await userService.DeleteUserAsync(parsedId);
        if (!success)
        {
            logger.LogWarning("User not found for deletion, ID: {UserId}", parsedId);
            throw new NotFoundException($"User with ID {parsedId} not found.");
        }

        logger.LogInformation("User {UserId} deleted successfully", parsedId);
        return NoContent();
    }
}
