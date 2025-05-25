using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OriginsController : ControllerBase
{
    private readonly IOriginService _originService;

    public OriginsController(IOriginService originService)
    {
        _originService = originService;
    }

    /// <summary>
    /// Retrieves all coffee origins
    /// </summary>
    /// <returns>List of all origins</returns>
    [HttpGet]
    public ActionResult<IEnumerable<OriginDto>> GetAll()
    {
        var origins = _originService.GetAllOrigins();
        return Ok(origins);
    }

    /// <summary>
    /// Retrieves a specific origin by its ID
    /// </summary>
    /// <param name="id">The ID of the origin to retrieve</param>
    /// <returns>The requested origin or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<OriginDto> GetById(int id)
    {
        var origin = _originService.GetOriginById(id);
        if (origin is null) return NotFound();
        return Ok(origin);
    }

    /// <summary>
    /// Creates a new coffee origin
    /// </summary>
    /// <param name="createDto">The origin data to create</param>
    /// <returns>The created origin with its new ID</returns>
    [HttpPost]
    public ActionResult<OriginDto> Create(CreateOriginDto createDto)
    {
        var origin = _originService.CreateOrigin(createDto);
        return CreatedAtAction(nameof(GetById), new { id = origin.Id }, origin);
    }

    /// <summary>
    /// Updates an existing origin by ID
    /// </summary>
    /// <param name="id">ID of the origin to update</param>
    /// <param name="updateDto">New origin data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateOriginDto updateDto)
    {
        var success = _originService.UpdateOrigin(id, updateDto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes an origin by its ID
    /// </summary>
    /// <param name="id">ID of the origin to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var success = _originService.DeleteOrigin(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
