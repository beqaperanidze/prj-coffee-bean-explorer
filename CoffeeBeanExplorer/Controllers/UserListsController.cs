using System.Security.Claims;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-lists")]
[Authorize]
public class UserListController(IUserListService userListService) : ControllerBase
{
    /// <summary>
    ///     Retrieves all user lists
    /// </summary>
    /// <returns>List of all user lists</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetAll()
    {
        var lists = await userListService.GetAllListsAsync();
        return Ok(lists);
    }

    /// <summary>
    ///     Retrieves a specific user list by its ID
    /// </summary>
    /// <param name="id">The ID of the user list to retrieve</param>
    /// <returns>The requested user list or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserListDto>> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var list = await userListService.GetListByIdAsync(parsedId);
        if (list is null) return NotFound();
        return Ok(list);
    }

    /// <summary>
    ///     Retrieves all lists for a specific user
    /// </summary>
    /// <param name="userId">ID of the user to get lists for</param>
    /// <returns>List of user lists for the specified user</returns>
    [HttpGet("users/{userId}")]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetUserLists(string userId)
    {
        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest("Invalid user ID format or value too large.");

        var lists = await userListService.GetListsByUserIdAsync(parsedUserId);
        return Ok(lists);
    }

    /// <summary>
    ///     Creates a new user list for the authenticated user
    /// </summary>
    /// <param name="dto">The user list data to create</param>
    /// <returns>The created user list with its new ID</returns>
    [HttpPost]
    public async Task<ActionResult<UserListDto>> Create([FromBody] CreateUserListDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var list = await userListService.CreateListAsync(dto, currentParsedId);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
    }

    /// <summary>
    ///     Updates an existing user list by ID
    /// </summary>
    /// <param name="id">ID of the list to update</param>
    /// <param name="dto">New list data</param>
    /// <returns>The updated user list or NotFound</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserListDto>> Update(string id, [FromBody] UpdateUserListDto dto)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid list ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdmin = User.IsInRole("Admin");

        var list = await userListService.GetListByIdAsync(parsedId);
        if (list == null)
            return NotFound();

        if (list.UserId != currentParsedId && !isAdmin)
            return Forbid("You can only update your own lists.");

        var updatedList = await userListService.UpdateListAsync(parsedId, dto, currentParsedId);
        if (updatedList is null) return NotFound();
        return Ok(updatedList);
    }

    /// <summary>
    ///     Deletes a user list by its ID
    /// </summary>
    /// <param name="id">ID of the list to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid list ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdmin = User.IsInRole("Admin");

        var list = await userListService.GetListByIdAsync(parsedId);
        if (list == null)
            return NotFound();

        if (list.UserId != currentParsedId && !isAdmin)
            return Forbid("You can only delete your own lists.");

        var success = await userListService.DeleteListAsync(parsedId, currentParsedId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Adds a bean to a user list
    /// </summary>
    /// <param name="listId">ID of the list to add the bean to</param>
    /// <param name="beanId">ID of the bean to add</param>
    /// <returns>No content on success</returns>
    [HttpPost("{listId}/beans/{beanId}")]
    public async Task<ActionResult> AddBeanToList(string listId, string beanId)
    {
        if (!int.TryParse(listId, out var parsedListId))
            return BadRequest("Invalid list ID format or value too large.");

        if (!int.TryParse(beanId, out var parsedBeanId))
            return BadRequest("Invalid bean ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdmin = User.IsInRole("Admin");

        var list = await userListService.GetListByIdAsync(parsedListId);
        if (list == null)
            return NotFound();

        if (list.UserId != currentParsedId && !isAdmin)
            return Forbid("You can only modify your own lists.");

        var success = await userListService.AddBeanToListAsync(parsedListId, parsedBeanId, currentParsedId);
        if (!success) return BadRequest();
        return NoContent();
    }

    /// <summary>
    ///     Removes a bean from a user list
    /// </summary>
    /// <param name="listId">ID of the list to remove the bean from</param>
    /// <param name="beanId">ID of the bean to remove</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{listId}/beans/{beanId}")]
    public async Task<ActionResult> RemoveBeanFromList(string listId, string beanId)
    {
        if (!int.TryParse(listId, out var parsedListId))
            return BadRequest("Invalid list ID format or value too large.");

        if (!int.TryParse(beanId, out var parsedBeanId))
            return BadRequest("Invalid bean ID format or value too large.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
            return Unauthorized("User is not authenticated or has invalid ID.");

        var isAdmin = User.IsInRole("Admin");

        var list = await userListService.GetListByIdAsync(parsedListId);
        if (list == null)
            return NotFound();

        if (list.UserId != currentParsedId && !isAdmin)
            return Forbid("You can only modify your own lists.");

        var success = await userListService.RemoveBeanFromListAsync(parsedListId, parsedBeanId, currentParsedId);
        if (!success) return NotFound();
        return NoContent();
    }
}
