using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/beans")]
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
    public async Task<ActionResult<BeanDto>> Create(CreateBeanDto dto)
    {
        var bean = await beanService.CreateBeanAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = bean.Id }, bean);
    }

    /// <summary>
    ///     Update an existing bean.
    /// </summary>
    [HttpPut("{id}")]
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
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var success = await beanService.DeleteBeanAsync(parsedId);
        if (!success) return NotFound();
        return NoContent();
    }
}
