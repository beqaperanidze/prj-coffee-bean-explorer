using System.Collections.Generic;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    /// <summary>
    /// Retrieves all tags
    /// </summary>
    /// <returns>List of all tags</returns>
    [HttpGet]
    public ActionResult<IEnumerable<TagDto>> GetAll()
    {
        var tags = _tagService.GetAllTags();
        return Ok(tags);
    }

    /// <summary>
    /// Retrieves a specific tag by its ID
    /// </summary>
    /// <param name="id">The ID of the tag to retrieve</param>
    /// <returns>The requested tag or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<TagDto> GetById(int id)
    {
        var tag = _tagService.GetTagById(id);
        if (tag is null) return NotFound();
        return Ok(tag);
    }

    /// <summary>
    /// Retrieves all tags associated with a specific bean
    /// </summary>
    /// <param name="beanId">The bean ID to get tags for</param>
    /// <returns>List of tags for the specified bean</returns>
    [HttpGet("byBean/{beanId:int}")]
    public ActionResult<IEnumerable<TagDto>> GetByBeanId(int beanId)
    {
        var tags = _tagService.GetTagsByBeanId(beanId);
        return Ok(tags);
    }

    /// <summary>
    /// Creates a new tag
    /// </summary>
    /// <param name="dto">The tag data to create</param>
    /// <returns>The created tag with its new ID</returns>
    [HttpPost]
    public ActionResult<TagDto> Create(CreateTagDto dto)
    {
        var tag = _tagService.CreateTag(dto);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
    }

    /// <summary>
    /// Updates an existing tag by ID
    /// </summary>
    /// <param name="id">ID of the tag to update</param>
    /// <param name="dto">New tag data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateTagDto dto)
    {
        var success = _tagService.UpdateTag(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a tag by its ID
    /// </summary>
    /// <param name="id">ID of the tag to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var success = _tagService.DeleteTag(id);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Associates a tag with a bean
    /// </summary>
    /// <param name="tagId">ID of the tag</param>
    /// <param name="beanId">ID of the bean</param>
    /// <returns>No content on success</returns>
    [HttpPost("{tagId:int}/beans/{beanId:int}")]
    public IActionResult AddTagToBean(int tagId, int beanId)
    {
        var success = _tagService.AddTagToBean(tagId, beanId);
        if (!success) return BadRequest();
        return NoContent();
    }

    /// <summary>
    /// Removes a tag from a bean
    /// </summary>
    /// <param name="tagId">ID of the tag</param>
    /// <param name="beanId">ID of the bean</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{tagId:int}/beans/{beanId:int}")]
    public IActionResult RemoveTagFromBean(int tagId, int beanId)
    {
        var success = _tagService.RemoveTagFromBean(tagId, beanId);
        if (!success) return NotFound();
        return NoContent();
    }
}
