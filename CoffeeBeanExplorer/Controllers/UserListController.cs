using CoffeeBeanExplorer.Enums;
using CoffeeBeanExplorer.Models;
using CoffeeBeanExplorer.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserListController : ControllerBase
{
    private static readonly List<UserList> UserLists = [];
    private static int _nextId = 1;

    /// <summary>
    /// Retrieves all user list entries
    /// </summary>
    /// <returns>List of all entries</returns>
    [HttpGet]
    public ActionResult<IEnumerable<UserListDto>> GetAll()
    {
        var listDtos = UserLists.Select(MapToDto).ToList();
        return Ok(listDtos);
    }

    /// <summary>
    /// Retrieves a specific user list entry by its ID
    /// </summary>
    /// <param name="id">The ID of the entry to retrieve</param>
    /// <returns>The requested entry or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<UserListDto> GetById(int id)
    {
        var listEntry = UserLists.FirstOrDefault(l => l.Id == id);
        if (listEntry == null) return NotFound();
        return Ok(MapToDto(listEntry));
    }

    /// <summary>
    /// Retrieves all list entries for a specific user
    /// </summary>
    /// <param name="userId">The user ID to get entries for</param>
    /// <returns>List of entries for the specified user</returns>
    [HttpGet("byUser/{userId:int}")]
    public ActionResult<IEnumerable<UserListDto>> GetByUserId(int userId)
    {
        var entries = UserLists.Where(l => l.UserId == userId).Select(MapToDto).ToList();
        return Ok(entries);
    }

    /// <summary>
    /// Retrieves all list entries of a specific type for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="type">The collection type</param>
    /// <returns>List of entries of specified type for the user</returns>
    [HttpGet("byUser/{userId:int}/byType/{type}")]
    public ActionResult<IEnumerable<UserListDto>> GetByUserIdAndType(int userId, CollectionType type)
    {
        var entries = UserLists.Where(l => l.UserId == userId && l.Type == type).Select(MapToDto).ToList();
        return Ok(entries);
    }

    /// <summary>
    /// Creates a new user list entry
    /// </summary>
    /// <param name="createDto">The entry data to create</param>
    /// <returns>The created entry with its new ID</returns>
    [HttpPost]
    public ActionResult<UserListDto> Create(CreateUserListDto createDto)
    {
        if (UserLists.Any(l => l.UserId == createDto.UserId && l.BeanId == createDto.BeanId))
        {
            return BadRequest(new { Message = "User already has this bean in a collection" });
        }

        var listEntry = new UserList
        {
            Id = _nextId++,
            UserId = createDto.UserId,
            BeanId = createDto.BeanId,
            Type = createDto.Type,
            CreatedAt = DateTime.UtcNow,
            User = new User { Id = createDto.UserId }, 
            Bean = new Bean { Id = createDto.BeanId } 
        };

        UserLists.Add(listEntry);
        return CreatedAtAction(nameof(GetById), new { id = listEntry.Id }, MapToDto(listEntry));
    }

    /// <summary>
    /// Updates an existing user list entry by ID
    /// </summary>
    /// <param name="id">ID of the entry to update</param>
    /// <param name="updateDto">New entry data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateUserListDto updateDto)
    {
        var listEntry = UserLists.FirstOrDefault(l => l.Id == id);
        if (listEntry == null) return NotFound();

        listEntry.Type = updateDto.Type;

        return NoContent();
    }

    /// <summary>
    /// Deletes a user list entry by its ID
    /// </summary>
    /// <param name="id">ID of the entry to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var listEntry = UserLists.FirstOrDefault(l => l.Id == id);
        if (listEntry == null) return NotFound();

        UserLists.Remove(listEntry);
        return NoContent();
    }

    private static UserListDto MapToDto(UserList listEntry)
    {
        return new UserListDto
        {
            Id = listEntry.Id,
            UserId = listEntry.UserId,
            Username = listEntry.User.Username,
            BeanId = listEntry.BeanId,
            BeanName = listEntry.Bean.Name,
            Type = listEntry.Type,
            CreatedAt = listEntry.CreatedAt
        };
    }
}