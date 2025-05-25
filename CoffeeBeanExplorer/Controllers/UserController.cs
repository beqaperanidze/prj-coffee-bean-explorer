using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetAll()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }

    /// <summary>
    /// Retrieves a specific user by ID
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The requested user or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<UserDto> GetById(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="userRegistrationDto">The user data for registration</param>
    /// <returns>The created user with its new ID</returns>
    [HttpPost]
    public ActionResult<UserDto> Create(UserRegistrationDto userRegistrationDto)
    {
        try
        {
            var user = _userService.RegisterUser(userRegistrationDto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing user by ID
    /// </summary>
    /// <param name="id">ID of the user to update</param>
    /// <param name="userUpdateDto">New user data</param>
    /// <returns>The updated user data</returns>
    [HttpPut("{id:int}")]
    public ActionResult<UserDto> Update(int id, UserUpdateDto userUpdateDto)
    {
        try
        {
            var user = _userService.UpdateUser(id, userUpdateDto);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a user by ID
    /// </summary>
    /// <param name="id">ID of the user to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var success = _userService.DeleteUser(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
