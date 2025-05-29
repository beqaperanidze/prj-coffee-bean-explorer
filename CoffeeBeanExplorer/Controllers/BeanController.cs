using System.Collections.Generic;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BeanController : ControllerBase
{
    private readonly IBeanService _beanService;

    public BeanController(IBeanService beanService)
    {
        _beanService = beanService;
    }

    /// <summary>
    /// Get all coffee beans.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BeanDto>>> GetAll()
    {
        var beans = await _beanService.GetAllBeansAsync();
        return Ok(beans);
    }

    /// <summary>
    /// Get a bean by its ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BeanDto>> GetById(int id)
    {
        var bean = await _beanService.GetBeanByIdAsync(id);
        if (bean is null) return NotFound();
        return Ok(bean);
    }

    /// <summary>
    /// Create a new coffee bean.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BeanDto>> Create(CreateBeanDto dto)
    {
        var bean = await _beanService.CreateBeanAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = bean.Id }, bean);
    }

    /// <summary>
    /// Update an existing bean.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateBeanDto dto)
    {
        var success = await _beanService.UpdateBeanAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Delete a bean by its ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _beanService.DeleteBeanAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
