using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UserController(IUserService userService) : ControllerBase
{
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
    ///     Creates a new user
    /// </summary>
    /// <param name="userRegistrationDto">The user data for registration</param>
    /// <returns>The created user with its new ID</returns>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserRegistrationDto userRegistrationDto)
    {
        try
        {
            var user = await userService.RegisterUserAsync(userRegistrationDto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
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

        var success = await userService.DeleteUserAsync(parsedId);
        if (!success) return NotFound();
        return NoContent();
    }
}
