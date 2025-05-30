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
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BeanDto>> GetById(int id)
    {
        var bean = await beanService.GetBeanByIdAsync(id);
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
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateBeanDto dto)
    {
        var success = await beanService.UpdateBeanAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Delete a bean by its ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await beanService.DeleteBeanAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
