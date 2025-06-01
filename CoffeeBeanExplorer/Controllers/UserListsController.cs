using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-lists")]
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
    ///     Creates a new user list
    /// </summary>
    /// <param name="dto">The user list data to create</param>
    /// <param name="userId">ID of the user creating the list</param>
    /// <returns>The created user list with its new ID</returns>
    [HttpPost("users/{userId}")]
    public async Task<ActionResult<UserListDto>> Create([FromBody] CreateUserListDto dto, string userId)
    {
        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest("Invalid user ID format or value too large.");

        var list = await userListService.CreateListAsync(dto, parsedUserId);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
    }

    /// <summary>
    ///     Updates an existing user list by ID
    /// </summary>
    /// <param name="id">ID of the list to update</param>
    /// <param name="dto">New list data</param>
    /// <param name="userId">ID of the user updating the list</param>
    /// <returns>The updated user list or NotFound</returns>
    [HttpPut("{id}/users/{userId}")]
    public async Task<ActionResult<UserListDto>> Update(string id, [FromBody] UpdateUserListDto dto, string userId)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid list ID format or value too large.");

        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest("Invalid user ID format or value too large.");

        var list = await userListService.UpdateListAsync(parsedId, dto, parsedUserId);
        if (list is null) return NotFound();
        return Ok(list);
    }

    /// <summary>
    ///     Deletes a user list by its ID
    /// </summary>
    /// <param name="id">ID of the list to delete</param>
    /// <param name="userId">ID of the user who owns the list</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> Delete(string id, string userId)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid list ID format or value too large.");

        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest("Invalid user ID format or value too large.");

        var success = await userListService.DeleteListAsync(parsedId, parsedUserId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Adds a bean to a user list
    /// </summary>
    /// <param name="listId">ID of the list to add the bean to</param>
    /// <param name="beanId">ID of the bean to add</param>
    /// <param name="userId">ID of the user who owns the list</param>
    /// <returns>No content on success</returns>
    [HttpPost("{listId}/beans/{beanId}/users/{userId}")]
    public async Task<ActionResult> AddBeanToList(string listId, string beanId, string userId)
    {
        if (!int.TryParse(listId, out var parsedListId))
            return BadRequest("Invalid list ID format or value too large.");

        if (!int.TryParse(beanId, out var parsedBeanId))
            return BadRequest("Invalid bean ID format or value too large.");

        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest("Invalid user ID format or value too large.");

        var success = await userListService.AddBeanToListAsync(parsedListId, parsedBeanId, parsedUserId);
        if (!success) return BadRequest();
        return NoContent();
    }

    /// <summary>
    ///     Removes a bean from a user list
    /// </summary>
    /// <param name="listId">ID of the list to remove the bean from</param>
    /// <param name="beanId">ID of the bean to remove</param>
    /// <param name="userId">ID of the user who owns the list</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{listId}/beans/{beanId}/users/{userId}")]
    public async Task<ActionResult> RemoveBeanFromList(string listId, string beanId, string userId)
    {
        if (!int.TryParse(listId, out var parsedListId))
            return BadRequest("Invalid list ID format or value too large.");

        if (!int.TryParse(beanId, out var parsedBeanId))
            return BadRequest("Invalid bean ID format or value too large.");

        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest("Invalid user ID format or value too large.");

        var success = await userListService.RemoveBeanFromListAsync(parsedListId, parsedBeanId, parsedUserId);
        if (!success) return NotFound();
        return NoContent();
    }
}
