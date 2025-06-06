using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/beans")]
[Authorize]
public class BeanController(IBeanService beanService) : ControllerBase
{
    /// <summary>
    ///     Get all coffee beans.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BeanDto>>> GetAll()
    {
        var beans = await beanService.GetAllBeansAsync();
        return Ok(beans);
    }

    /// <summary>
    ///     Get a bean by its ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BeanDto>> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var bean = await beanService.GetBeanByIdAsync(parsedId);
        if (bean is null) return NotFound();
        return Ok(bean);
    }


    /// <summary>
    ///     Create a new coffee bean.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<BeanDto>> Create(CreateBeanDto dto)
    {
        var bean = await beanService.CreateBeanAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = bean.Id }, bean);
    }

    /// <summary>
    ///     Update an existing bean.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Update(string id, UpdateBeanDto dto)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var success = await beanService.UpdateBeanAsync(parsedId, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Delete a bean by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var success = await beanService.DeleteBeanAsync(parsedId);
        if (!success) return NotFound();
        return NoContent();
    }
}
