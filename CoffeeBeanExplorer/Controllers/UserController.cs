using System.Security.Claims;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
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
            return Unauthorized("User is not authenticated or has invalid ID.");

        var user = await userService.GetUserByIdAsync(parsedId);
        if (user == null) return NotFound("User not found.");

        return Ok(user);
    }

    /// <summary>
    ///     Retrieves all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await userService.GetAllUsersAsync();
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
            return BadRequest("Invalid ID format or value too large.");

        var user = await userService.GetUserByIdAsync(parsedId);
        if (user == null) return NotFound();
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
            return BadRequest("Invalid ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdmin = User.IsInRole("Admin");
        if (parsedId != currentParsedId && !isAdmin)
            return Forbid("You can only update your own user information.");

        try
        {
            var user = await userService.UpdateUserAsync(parsedId, userUpdateDto);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
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
            return BadRequest("Invalid ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdmin = User.IsInRole("Admin");
        if (parsedId != currentParsedId && !isAdmin)
            return Forbid("You can only delete your own user account.");

        var success = await userService.DeleteUserAsync(parsedId);
        if (!success) return NotFound();
        return NoContent();
    }
}
