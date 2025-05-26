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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Ok(tags);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag is null) return NotFound();
        return Ok(tag);
    }

    [HttpGet("beans/{beanId:int}")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetByBeanId(int beanId)
    {
        var tags = await _tagService.GetTagsByBeanIdAsync(beanId);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(CreateTagDto dto)
    {
        var tag = await _tagService.CreateTagAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTagDto dto)
    {
        var success = await _tagService.UpdateTagAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _tagService.DeleteTagAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("{tagId:int}/beans/{beanId:int}")]
    public async Task<IActionResult> AddTagToBean(int tagId, int beanId)
    {
        var success = await _tagService.AddTagToBeanAsync(tagId, beanId);
        if (!success) return BadRequest();
        return NoContent();
    }

    [HttpDelete("{tagId:int}/beans/{beanId:int}")]
    public async Task<IActionResult> RemoveTagFromBean(int tagId, int beanId)
    {
        var success = await _tagService.RemoveTagFromBeanAsync(tagId, beanId);
        if (!success) return NotFound();
        return NoContent();
    }
}
