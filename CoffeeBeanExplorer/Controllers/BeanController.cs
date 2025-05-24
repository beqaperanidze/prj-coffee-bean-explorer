using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Models;
using CoffeeBeanExplorer.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BeanController : ControllerBase
{
    private static readonly List<Bean> Beans = [];
    private static int _nextId = 1;

    /// <summary>
    /// Retrieves all coffee beans
    /// </summary>
    /// <returns>List of all beans</returns>
    [HttpGet]
    public ActionResult<IEnumerable<BeanDto>> GetAll()
    {
        var beanDtos = Beans.Select(MapToDto).ToList();
        return Ok(beanDtos);
    }

    /// <summary>
    /// Retrieves a specific beans by its ID
    /// </summary>
    /// <param name="id">The ID of the bean to retrieve</param>
    /// <returns>The requested bean or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<BeanDto> GetById(int id)
    {
        var bean = Beans.FirstOrDefault(b => b.Id == id);
        if (bean == null) return NotFound();
        return Ok(MapToDto(bean));
    }

    /// <summary>
    /// Creates a new coffee bean
    /// </summary>
    /// <param name="createDto">The bean data to create</param>
    /// <returns>The created bean with its new ID</returns>
    [HttpPost]
    public ActionResult<BeanDto> Create(CreateBeanDto createDto)
    {
        var origin = new Origin();

        var bean = new Bean
        {
            Id = _nextId++,
            Name = createDto.Name,
            OriginId = createDto.OriginId,
            RoastLevel = createDto.RoastLevel,
            Description = createDto.Description,
            Price = createDto.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Origin = origin
        };

        Beans.Add(bean);
        return CreatedAtAction(nameof(GetById), new { id = bean.Id }, MapToDto(bean));
    }

    /// <summary>
    /// Updates an existing bean by ID
    /// </summary>
    /// <param name="id">ID of the bean to update</param>
    /// <param name="updateDto">New bean data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateBeanDto updateDto)
    {
        var bean = Beans.FirstOrDefault(b => b.Id == id);
        if (bean == null) return NotFound();

        bean.Name = updateDto.Name;
        bean.OriginId = updateDto.OriginId;
        bean.RoastLevel = updateDto.RoastLevel;
        bean.Description = updateDto.Description;
        bean.Price = updateDto.Price;
        bean.UpdatedAt = DateTime.UtcNow;

        return NoContent();
    }

    /// <summary>
    /// Deletes a bean by its ID
    /// </summary>
    /// <param name="id">ID of the bean to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var bean = Beans.FirstOrDefault(b => b.Id == id);
        if (bean == null) return NotFound();

        Beans.Remove(bean);
        return NoContent();
    }

    private static BeanDto MapToDto(Bean bean)
    {
        return new BeanDto
        {
            Id = bean.Id,
            Name = bean.Name,
            OriginId = bean.OriginId,
            OriginCountry = bean.Origin.Country,
            OriginRegion = bean.Origin.Region,
            RoastLevel = bean.RoastLevel,
            Description = bean.Description,
            Price = bean.Price,
            CreatedAt = bean.CreatedAt,
            UpdatedAt = bean.UpdatedAt
        };
    }
}
