using System.Security.Claims;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-lists")]
[Authorize]
public class UserListController(IUserListService userListService, ILogger<UserListController> logger) : ControllerBase
{
    /// <summary>
    ///     Retrieves all user lists
    /// </summary>
    /// <returns>List of all user lists</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all user lists");
        var lists = await userListService.GetAllListsAsync();
        logger.LogInformation("Successfully retrieved {ListCount} user lists", lists.Count());
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
        {
            logger.LogWarning("Invalid list ID format: {ListId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Retrieving user list with ID: {ListId}", parsedId);
        var list = await userListService.GetListByIdAsync(parsedId);
        if (list is null)
        {
            logger.LogWarning("User list not found for ID: {ListId}", parsedId);
            throw new NotFoundException($"User list with ID {parsedId} not found.");
        }

        logger.LogInformation("User list retrieved successfully with ID: {ListId}", parsedId);
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
        {
            logger.LogWarning("Invalid user ID format: {UserId}", userId);
            throw new BadRequestException("Invalid user ID format or value too large.");
        }

        logger.LogInformation("Retrieving lists for user ID: {UserId}", parsedUserId);
        var lists = await userListService.GetListsByUserIdAsync(parsedUserId);
        logger.LogInformation("Retrieved {ListCount} lists for user ID: {UserId}", lists.Count(), parsedUserId);
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
        {
            logger.LogWarning("User authentication failed or invalid ID when creating user list");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        logger.LogInformation("Creating new user list with name: {Name} for user ID: {UserId}", dto.Name,
            currentParsedId);
        var list = await userListService.CreateListAsync(dto, currentParsedId);
        logger.LogInformation("Created new user list with ID: {ListId}", list.Id);
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
        {
            logger.LogWarning("Invalid list ID format: {ListId}", id);
            throw new BadRequestException("Invalid list ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID when updating list");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        var isAdmin = User.IsInRole("Admin");

        logger.LogInformation("Retrieving user list with ID: {ListId} for update", parsedId);
        var list = await userListService.GetListByIdAsync(parsedId);
        if (list == null)
        {
            logger.LogWarning("User list not found for update, ID: {ListId}", parsedId);
            throw new NotFoundException($"User list with ID {parsedId} not found.");
        }

        if (list.UserId != currentParsedId && !isAdmin)
        {
            logger.LogWarning("Unauthorized update attempt by user: {CurrentUserId} for list ID: {ListId}",
                currentParsedId, parsedId);
            throw new UnauthorizedException("You can only update your own lists.");
        }

        logger.LogInformation("Updating user list with ID: {ListId}", parsedId);
        var updatedList = await userListService.UpdateListAsync(parsedId, dto, currentParsedId);
        if (updatedList is null)
        {
            logger.LogWarning("User list not found for update, ID: {ListId}", parsedId);
            throw new NotFoundException($"User list with ID {parsedId} not found.");
        }

        logger.LogInformation("User list {ListId} updated successfully", parsedId);
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
        {
            logger.LogWarning("Invalid list ID format for deletion: {ListId}", id);
            throw new BadRequestException("Invalid list ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID when deleting list");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        var isAdmin = User.IsInRole("Admin");

        logger.LogInformation("Retrieving user list with ID: {ListId} for deletion", parsedId);
        var list = await userListService.GetListByIdAsync(parsedId);
        if (list == null)
        {
            logger.LogWarning("User list not found for deletion, ID: {ListId}", parsedId);
            throw new NotFoundException($"User list with ID {parsedId} not found.");
        }

        if (list.UserId != currentParsedId && !isAdmin)
        {
            logger.LogWarning("Unauthorized deletion attempt by user: {CurrentUserId} for list ID: {ListId}",
                currentParsedId, parsedId);
            throw new UnauthorizedException("You can only delete your own lists.");
        }

        logger.LogInformation("Deleting user list with ID: {ListId}", parsedId);
        var success = await userListService.DeleteListAsync(parsedId, currentParsedId);
        if (!success)
        {
            logger.LogWarning("Failed to delete user list with ID: {ListId}", parsedId);
            throw new NotFoundException($"User list with ID {parsedId} not found.");
        }

        logger.LogInformation("User list {ListId} deleted successfully", parsedId);
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
        {
            logger.LogWarning("Invalid list ID format: {ListId}", listId);
            throw new BadRequestException("Invalid list ID format or value too large.");
        }

        if (!int.TryParse(beanId, out var parsedBeanId))
        {
            logger.LogWarning("Invalid bean ID format: {BeanId}", beanId);
            throw new BadRequestException("Invalid bean ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID when adding bean to list");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        var isAdmin = User.IsInRole("Admin");

        logger.LogInformation("Retrieving user list with ID: {ListId} to add bean", parsedListId);
        var list = await userListService.GetListByIdAsync(parsedListId);
        if (list == null)
        {
            logger.LogWarning("User list not found, ID: {ListId}", parsedListId);
            throw new NotFoundException($"User list with ID {parsedListId} not found.");
        }

        if (list.UserId != currentParsedId && !isAdmin)
        {
            logger.LogWarning("Unauthorized attempt by user: {CurrentUserId} to modify list ID: {ListId}",
                currentParsedId, parsedListId);
            throw new UnauthorizedException("You can only modify your own lists.");
        }

        logger.LogInformation("Attempting to add bean ID: {BeanId} to list ID: {ListId}", parsedBeanId, parsedListId);
        var success = await userListService.AddBeanToListAsync(parsedListId, parsedBeanId, currentParsedId);
        if (!success)
        {
            logger.LogWarning("Failed to add bean ID: {BeanId} to list ID: {ListId}", parsedBeanId, parsedListId);
            throw new BadRequestException("Bean or list not found, or bean already exists in the list.");
        }

        logger.LogInformation("Successfully added bean ID: {BeanId} to list ID: {ListId}", parsedBeanId, parsedListId);
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
        {
            logger.LogWarning("Invalid list ID format: {ListId}", listId);
            throw new BadRequestException("Invalid list ID format or value too large.");
        }

        if (!int.TryParse(beanId, out var parsedBeanId))
        {
            logger.LogWarning("Invalid bean ID format: {BeanId}", beanId);
            throw new BadRequestException("Invalid bean ID format or value too large.");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var currentParsedId))
        {
            logger.LogWarning("User authentication failed or invalid ID when removing bean from list");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        logger.LogInformation("Attempting to remove bean ID: {BeanId} from list ID: {ListId}", parsedBeanId,
            parsedListId);
        var success = await userListService.RemoveBeanFromListAsync(parsedListId, parsedBeanId, currentParsedId);
        if (!success)
        {
            logger.LogWarning("Failed to remove bean ID: {BeanId} from list ID: {ListId}", parsedBeanId, parsedListId);
            throw new BadRequestException(
                "Bean or list not found, association doesn't exist, or you don't have permission.");
        }

        logger.LogInformation("Successfully removed bean ID: {BeanId} from list ID: {ListId}", parsedBeanId,
            parsedListId);
        return NoContent();
    }
}
