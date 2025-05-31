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
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserListDto>> GetById(int id)
    {
        var list = await userListService.GetListByIdAsync(id);
        if (list is null) return NotFound();
        return Ok(list);
    }

    /// <summary>
    ///     Retrieves all lists for a specific user
    /// </summary>
    /// <param name="userId">ID of the user to get lists for</param>
    /// <returns>List of user lists for the specified user</returns>
    [HttpGet("users/{userId:int}")]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetUserLists(int userId)
    {
        var lists = await userListService.GetListsByUserIdAsync(userId);
        return Ok(lists);
    }

    /// <summary>
    ///     Creates a new user list
    /// </summary>
    /// <param name="dto">The user list data to create</param>
    /// <param name="userId">ID of the user creating the list</param>
    /// <returns>The created user list with its new ID</returns>
    [HttpPost("users/{userId:int}")]
    public async Task<ActionResult<UserListDto>> Create([FromBody] CreateUserListDto dto, int userId)
    {
        var list = await userListService.CreateListAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
    }

    /// <summary>
    ///     Updates an existing user list by ID
    /// </summary>
    /// <param name="id">ID of the list to update</param>
    /// <param name="dto">New list data</param>
    /// <param name="userId">ID of the user updating the list</param>
    /// <returns>The updated user list or NotFound</returns>
    [HttpPut("{id:int}/users/{userId:int}")]
    public async Task<ActionResult<UserListDto>> Update(int id, [FromBody] UpdateUserListDto dto, int userId)
    {
        var list = await userListService.UpdateListAsync(id, dto, userId);
        if (list is null) return NotFound();
        return Ok(list);
    }

    /// <summary>
    ///     Deletes a user list by its ID
    /// </summary>
    /// <param name="id">ID of the list to delete</param>
    /// <param name="userId">ID of the user who owns the list</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}/users/{userId:int}")]
    public async Task<ActionResult> Delete(int id, int userId)
    {
        var success = await userListService.DeleteListAsync(id, userId);
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
    [HttpPost("{listId:int}/beans/{beanId:int}/users/{userId:int}")]
    public async Task<ActionResult> AddBeanToList(int listId, int beanId, int userId)
    {
        var success = await userListService.AddBeanToListAsync(listId, beanId, userId);
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
    [HttpDelete("{listId:int}/beans/{beanId:int}/users/{userId:int}")]
    public async Task<ActionResult> RemoveBeanFromList(int listId, int beanId, int userId)
    {
        var success = await userListService.RemoveBeanFromListAsync(listId, beanId, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}
