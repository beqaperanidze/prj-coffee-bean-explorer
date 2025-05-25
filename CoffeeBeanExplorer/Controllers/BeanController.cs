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
    public ActionResult<IEnumerable<BeanDto>> GetAll()
    {
        var beans = _beanService.GetAllBeans();
        return Ok(beans);
    }

    /// <summary>
    /// Get a bean by its ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<BeanDto> GetById(int id)
    {
        var bean = _beanService.GetBeanById(id);
        if (bean is null) return NotFound();
        return Ok(bean);
    }

    /// <summary>
    /// Create a new coffee bean.
    /// </summary>
    [HttpPost]
    public ActionResult<BeanDto> Create(CreateBeanDto dto)
    {
        var bean = _beanService.CreateBean(dto);
        return CreatedAtAction(nameof(GetById), new { id = bean.Id }, bean);
    }

    /// <summary>
    /// Update an existing bean.
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateBeanDto dto)
    {
        var success = _beanService.UpdateBean(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Delete a bean by its ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var success = _beanService.DeleteBean(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
