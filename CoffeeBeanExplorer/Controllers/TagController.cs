using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Enums;
using CoffeeBeanExplorer.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tags")]
[Authorize]
public class TagController(ITagService tagService, ILogger<TagController> logger) : ControllerBase
{
    /// <summary>
    ///     Retrieves all tags
    /// </summary>
    /// <returns>List of all tags</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all tags");
        var tags = await tagService.GetAllTagsAsync();
        logger.LogInformation("Successfully retrieved {TagCount} tags", tags.Count());
        return Ok(tags);
    }

    /// <summary>
    ///     Retrieves a specific tag by its ID
    /// </summary>
    /// <param name="id">The ID of the tag to retrieve</param>
    /// <returns>The requested tag or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid tag ID format: {TagId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Retrieving tag with ID: {TagId}", parsedId);
        var tag = await tagService.GetTagByIdAsync(parsedId);
        if (tag is null)
        {
            logger.LogWarning("Tag not found for ID: {TagId}", parsedId);
            throw new NotFoundException($"Tag with ID {parsedId} not found.");
        }

        logger.LogInformation("Tag retrieved successfully with ID: {TagId}", parsedId);
        return Ok(tag);
    }

    /// <summary>
    ///     Retrieves all tags for a specific bean
    /// </summary>
    /// <param name="beanId">ID of the bean to get tags for</param>
    /// <returns>List of tags for the specified bean</returns>
    [HttpGet("beans/{beanId}")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetByBeanId(string beanId)
    {
        if (!int.TryParse(beanId, out var parsedBeanId))
        {
            logger.LogWarning("Invalid bean ID format: {BeanId}", beanId);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Retrieving tags for bean ID: {BeanId}", parsedBeanId);
        var tags = await tagService.GetTagsByBeanIdAsync(parsedBeanId);
        logger.LogInformation("Retrieved {TagCount} tags for bean ID: {BeanId}", tags.Count(), parsedBeanId);
        return Ok(tags);
    }

    /// <summary>
    ///     Retrieves all beans that have a specific tag
    /// </summary>
    /// <param name="tagId">ID of the tag to filter beans by</param>
    /// <returns>List of beans that have the specified tag</returns>
    [HttpGet("{tagId}/beans")]
    public async Task<ActionResult<IEnumerable<BeanDto>>> GetBeansByTagId(string tagId)
    {
        if (!int.TryParse(tagId, out var parsedTagId))
        {
            logger.LogWarning("Invalid tag ID format: {TagId}", tagId);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Retrieving beans for tag ID: {TagId}", parsedTagId);
        var beans = await tagService.GetBeansByTagIdAsync(parsedTagId);
        logger.LogInformation("Retrieved {BeanCount} beans for tag ID: {TagId}", beans.Count(), parsedTagId);
        return Ok(beans);
    }

    /// <summary>
    ///     Creates a new tag
    /// </summary>
    /// <param name="dto">The tag data to create</param>
    /// <returns>The created tag with its new ID</returns>
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<TagDto>> Create(CreateTagDto dto)
    {
        try
        {
            logger.LogInformation("Creating new tag with name: {TagName}", dto.Name);
            var tag = await tagService.CreateTagAsync(dto);
            logger.LogInformation("Created new tag with ID: {TagId}", tag.Id);
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating tag");
            throw new InternalServerErrorException($"An error occurred while creating tag: {ex.Message}");
        }
    }

    /// <summary>
    ///     Updates an existing tag by ID
    /// </summary>
    /// <param name="id">ID of the tag to update</param>
    /// <param name="dto">New tag data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Update(string id, UpdateTagDto dto)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid tag ID format for update: {TagId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Updating tag with ID: {TagId}", parsedId);
        var success = await tagService.UpdateTagAsync(parsedId, dto);
        if (!success)
        {
            logger.LogWarning("Tag not found for update, ID: {TagId}", parsedId);
            throw new NotFoundException($"Tag with ID {parsedId} not found.");
        }

        logger.LogInformation("Tag {TagId} updated successfully", parsedId);
        return NoContent();
    }

    /// <summary>
    ///     Deletes a tag by its ID
    /// </summary>
    /// <param name="id">ID of the tag to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid tag ID format for deletion: {TagId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Attempting to delete tag with ID: {TagId}", parsedId);
        var success = await tagService.DeleteTagAsync(parsedId);
        if (!success)
        {
            logger.LogWarning("Tag not found for deletion, ID: {TagId}", parsedId);
            throw new NotFoundException($"Tag with ID {parsedId} not found.");
        }

        logger.LogInformation("Tag deleted successfully, ID: {TagId}", parsedId);
        return NoContent();
    }

    /// <summary>
    ///     Associates a tag with a bean
    /// </summary>
    /// <param name="tagId">ID of the tag to associate</param>
    /// <param name="beanId">ID of the bean to tag</param>
    /// <returns>No content on success</returns>
    [HttpPost("{tagId}/beans/{beanId}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> AddTagToBean(string tagId, string beanId)
    {
        if (!int.TryParse(tagId, out var parsedTagId))
        {
            logger.LogWarning("Invalid tag ID format: {TagId}", tagId);
            throw new BadRequestException("Invalid tag ID format or value too large.");
        }

        if (!int.TryParse(beanId, out var parsedBeanId))
        {
            logger.LogWarning("Invalid bean ID format: {BeanId}", beanId);
            throw new BadRequestException("Invalid bean ID format or value too large.");
        }

        logger.LogInformation("Attempting to associate tag ID: {TagId} with bean ID: {BeanId}", parsedTagId,
            parsedBeanId);
        var success = await tagService.AddTagToBeanAsync(parsedTagId, parsedBeanId);
        if (!success)
        {
            logger.LogWarning("Failed to associate tag ID: {TagId} with bean ID: {BeanId}", parsedTagId, parsedBeanId);
            throw new BadRequestException("Tag or bean not found, or association already exists.");
        }

        logger.LogInformation("Tag ID: {TagId} successfully associated with bean ID: {BeanId}", parsedTagId,
            parsedBeanId);
        return NoContent();
    }

    /// <summary>
    ///     Removes a tag association from a bean
    /// </summary>
    /// <param name="tagId">ID of the tag to remove</param>
    /// <param name="beanId">ID of the bean to remove tag from</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{tagId}/beans/{beanId}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> RemoveTagFromBean(string tagId, string beanId)
    {
        if (!int.TryParse(tagId, out var parsedTagId))
        {
            logger.LogWarning("Invalid tag ID format: {TagId}", tagId);
            throw new BadRequestException("Invalid tag ID format or value too large.");
        }

        if (!int.TryParse(beanId, out var parsedBeanId))
        {
            logger.LogWarning("Invalid bean ID format: {BeanId}", beanId);
            throw new BadRequestException("Invalid bean ID format or value too large.");
        }

        logger.LogInformation("Attempting to remove association of tag ID: {TagId} from bean ID: {BeanId}", parsedTagId,
            parsedBeanId);
        var success = await tagService.RemoveTagFromBeanAsync(parsedTagId, parsedBeanId);
        if (!success)
        {
            logger.LogWarning("Failed to remove association of tag ID: {TagId} from bean ID: {BeanId}", parsedTagId,
                parsedBeanId);
            throw new BadRequestException("Tag or bean not found, or association doesn't exist.");
        }

        logger.LogInformation("Association of tag ID: {TagId} successfully removed from bean ID: {BeanId}", parsedTagId,
            parsedBeanId);
        return NoContent();
    }
}
