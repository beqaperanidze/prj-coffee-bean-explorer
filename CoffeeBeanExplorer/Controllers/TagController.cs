using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tags")]
public class TagController(ITagService tagService) : ControllerBase
{
    /// <summary>
    ///     Retrieves all tags
    /// </summary>
    /// <returns>List of all tags</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        var tags = await tagService.GetAllTagsAsync();
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
            return BadRequest("Invalid ID format or value too large.");

        var tag = await tagService.GetTagByIdAsync(parsedId);
        if (tag is null) return NotFound();
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
            return BadRequest("Invalid ID format or value too large.");

        var tags = await tagService.GetTagsByBeanIdAsync(parsedBeanId);
        return Ok(tags);
    }

    /// <summary>
    ///     Creates a new tag
    /// </summary>
    /// <param name="dto">The tag data to create</param>
    /// <returns>The created tag with its new ID</returns>
    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(CreateTagDto dto)
    {
        var tag = await tagService.CreateTagAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
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
            return BadRequest("Invalid ID format or value too large.");

        var beans = await tagService.GetBeansByTagIdAsync(parsedTagId);
        return Ok(beans);
    }

    /// <summary>
    ///     Updates an existing tag by ID
    /// </summary>
    /// <param name="id">ID of the tag to update</param>
    /// <param name="dto">New tag data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateTagDto dto)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var success = await tagService.UpdateTagAsync(parsedId, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Deletes a tag by its ID
    /// </summary>
    /// <param name="id">ID of the tag to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var success = await tagService.DeleteTagAsync(parsedId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Associates a tag with a bean
    /// </summary>
    /// <param name="tagId">ID of the tag to associate</param>
    /// <param name="beanId">ID of the bean to tag</param>
    /// <returns>No content on success</returns>
    [HttpPost("{tagId}/beans/{beanId}")]
    public async Task<IActionResult> AddTagToBean(string tagId, string beanId)
    {
        if (!int.TryParse(tagId, out var parsedTagId))
            return BadRequest("Invalid tag ID format or value too large.");

        if (!int.TryParse(beanId, out var parsedBeanId))
            return BadRequest("Invalid bean ID format or value too large.");

        var success = await tagService.AddTagToBeanAsync(parsedTagId, parsedBeanId);
        if (!success) return BadRequest();
        return NoContent();
    }

    /// <summary>
    ///     Removes a tag association from a bean
    /// </summary>
    /// <param name="tagId">ID of the tag to remove</param>
    /// <param name="beanId">ID of the bean to remove tag from</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{tagId}/beans/{beanId}")]
    public async Task<IActionResult> RemoveTagFromBean(string tagId, string beanId)
    {
        if (!int.TryParse(tagId, out var parsedTagId))
            return BadRequest("Invalid tag ID format or value too large.");

        if (!int.TryParse(beanId, out var parsedBeanId))
            return BadRequest("Invalid bean ID format or value too large.");

        var success = await tagService.RemoveTagFromBeanAsync(parsedTagId, parsedBeanId);
        if (!success) return NotFound();
        return NoContent();
    }
}
