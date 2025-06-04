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
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        var tag = await tagService.GetTagByIdAsync(id);
        if (tag is null) return NotFound();
        return Ok(tag);
    }

    /// <summary>
    ///     Retrieves all tags for a specific bean
    /// </summary>
    /// <param name="beanId">ID of the bean to get tags for</param>
    /// <returns>List of tags for the specified bean</returns>
    [HttpGet("beans/{beanId:int}")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetByBeanId(int beanId)
    {
        var tags = await tagService.GetTagsByBeanIdAsync(beanId);
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
    [HttpGet("{tagId:int}/beans")]
    public async Task<ActionResult<IEnumerable<BeanDto>>> GetBeansByTagId(int tagId)
    {
        var beans = await tagService.GetBeansByTagIdAsync(tagId);
        return Ok(beans);
    }

    /// <summary>
    ///     Updates an existing tag by ID
    /// </summary>
    /// <param name="id">ID of the tag to update</param>
    /// <param name="dto">New tag data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTagDto dto)
    {
        var success = await tagService.UpdateTagAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Deletes a tag by its ID
    /// </summary>
    /// <param name="id">ID of the tag to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await tagService.DeleteTagAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Associates a tag with a bean
    /// </summary>
    /// <param name="tagId">ID of the tag to associate</param>
    /// <param name="beanId">ID of the bean to tag</param>
    /// <returns>No content on success</returns>
    [HttpPost("{tagId:int}/beans/{beanId:int}")]
    public async Task<IActionResult> AddTagToBean(int tagId, int beanId)
    {
        var success = await tagService.AddTagToBeanAsync(tagId, beanId);
        if (!success) return BadRequest();
        return NoContent();
    }

    /// <summary>
    ///     Removes a tag association from a bean
    /// </summary>
    /// <param name="tagId">ID of the tag to remove</param>
    /// <param name="beanId">ID of the bean to remove tag from</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{tagId:int}/beans/{beanId:int}")]
    public async Task<IActionResult> RemoveTagFromBean(int tagId, int beanId)
    {
        var success = await tagService.RemoveTagFromBeanAsync(tagId, beanId);
        if (!success) return NotFound();
        return NoContent();
    }
}
