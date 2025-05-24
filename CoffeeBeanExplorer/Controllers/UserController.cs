using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Models;
using CoffeeBeanExplorer.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController : ControllerBase
{
    private static readonly List<User> Users = [];
    private static int _nextId = 1;

    /// <summary>
    /// Retrieves all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetAll()
    {
        var userDtos = Users.Select(MapToDto).ToList();
        return Ok(userDtos);
    }

    /// <summary>
    /// Retrieves a specific user by ID
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The requested user or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<UserDto> GetById(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        return MapToDto(user);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="userRegistrationDto">The user data for registration</param>
    /// <returns>The created user with its new ID</returns>
    [HttpPost]
    public ActionResult<UserDto> Create(UserRegistrationDto userRegistrationDto)
    {
        if (Users.Any(u => u.Username == userRegistrationDto.Username))
        {
            return BadRequest(new { Message = "Username is already taken" });
        }

        if (Users.Any(u => u.Email == userRegistrationDto.Email))
        {
            return BadRequest(new { Message = "Email is already registered" });
        }

        var user = new User
        {
            Id = _nextId++,
            Username = userRegistrationDto.Username,
            Email = userRegistrationDto.Email,
            FirstName = userRegistrationDto.FirstName,
            LastName = userRegistrationDto.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Users.Add(user);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, MapToDto(user));
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
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        if (userUpdateDto.Username != null && user.Username != userUpdateDto.Username)
        {
            if (Users.Any(u => u.Username == userUpdateDto.Username))
            {
                return BadRequest(new { Message = "Username is already taken" });
            }

            user.Username = userUpdateDto.Username;
        }

        if (userUpdateDto.Email != null && user.Email != userUpdateDto.Email)
        {
            if (Users.Any(u => u.Email == userUpdateDto.Email))
            {
                return BadRequest(new { Message = "Email is already registered" });
            }

            user.Email = userUpdateDto.Email;
        }

        if (userUpdateDto.FirstName != null)
            user.FirstName = userUpdateDto.FirstName;

        if (userUpdateDto.LastName != null)
            user.LastName = userUpdateDto.LastName;

        user.UpdatedAt = DateTime.UtcNow;

        return Ok(MapToDto(user));
    }

    /// <summary>
    /// Deletes a user by ID
    /// </summary>
    /// <param name="id">ID of the user to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public ActionResult<UserDto> Delete(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        Users.Remove(user);
        return NoContent();
    }

    /// <summary>
    /// Maps a User entity to UserDto
    /// </summary>
    /// <param name="user">User entity to convert</param>
    /// <returns>UserDto without sensitive information</returns>
    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Role = user.Role
        };
    }
}
