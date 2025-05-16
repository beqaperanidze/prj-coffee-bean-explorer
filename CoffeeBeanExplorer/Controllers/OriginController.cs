using Microsoft.AspNetCore.Mvc;
using CoffeeBeanExplorer.Models;
using CoffeeBeanExplorer.Models.DTOs;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OriginsController : ControllerBase
{
    private static readonly List<Origin> Origins = [];
    private static int _nextId = 1;

    /// <summary>
    /// Retrieves all coffee origins
    /// </summary>
    /// <returns>List of all origins</returns>
    [HttpGet]
    public ActionResult<IEnumerable<OriginDto>> GetAll()
    {
        var originDtos = Origins.Select(MapToDto).ToList();
        return Ok(originDtos);
    }

    /// <summary>
    /// Retrieves a specific origin by its ID
    /// </summary>
    /// <param name="id">The ID of the origin to retrieve</param>
    /// <returns>The requested origin or NotFound</returns>
    [HttpGet("{id:int}")]
    public ActionResult<OriginDto> GetById(int id)
    {
        var origin = Origins.FirstOrDefault(o => o.Id == id);
        if (origin == null) return NotFound();
        return Ok(MapToDto(origin));
    }

    /// <summary>
    /// Creates a new coffee origin
    /// </summary>
    /// <param name="createDto">The origin data to create</param>
    /// <returns>The created origin with its new ID</returns>
    [HttpPost]
    public ActionResult<OriginDto> Create(CreateOriginDto createDto)
    {
        var origin = new Origin
        {
            Id = _nextId++,
            Country = createDto.Country,
            Region = createDto.Region,
            CreatedAt = DateTime.UtcNow,
            UpdDateTime = DateTime.UtcNow
        };

        Origins.Add(origin);
        return CreatedAtAction(nameof(GetById), new { id = origin.Id }, MapToDto(origin));
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
        var origin = Origins.FirstOrDefault(o => o.Id == id);
        if (origin == null) return NotFound();

        origin.Country = updateDto.Country;
        origin.Region = updateDto.Region;
        origin.UpdDateTime = DateTime.UtcNow;

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
        var origin = Origins.FirstOrDefault(o => o.Id == id);
        if (origin == null) return NotFound();

        Origins.Remove(origin);
        return NoContent();
    }

    private static OriginDto MapToDto(Origin origin)
    {
        return new OriginDto
        {
            Id = origin.Id,
            Country = origin.Country,
            Region = origin.Region,
            CreatedAt = origin.CreatedAt,
            UpdDateTime = origin.UpdDateTime
        };
    }
}