using System.Collections.Generic;
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
    public async Task<ActionResult<IEnumerable<OriginDto>>> GetAll()
    {
        var origins = await _originService.GetAllOriginsAsync();
        return Ok(origins);
    }

    /// <summary>
    /// Retrieves a specific origin by its ID
    /// </summary>
    /// <param name="id">The ID of the origin to retrieve</param>
    /// <returns>The requested origin or NotFound</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OriginDto>> GetById(int id)
    {
        var origin = await _originService.GetOriginByIdAsync(id);
        if (origin is null) return NotFound();
        return Ok(origin);
    }

    /// <summary>
    /// Creates a new coffee origin
    /// </summary>
    /// <param name="createDto">The origin data to create</param>
    /// <returns>The created origin with its new ID</returns>
    [HttpPost]
    public async Task<ActionResult<OriginDto>> Create(CreateOriginDto createDto)
    {
        var origin = await _originService.CreateOriginAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = origin.Id }, origin);
    }

    /// <summary>
    /// Updates an existing origin by ID
    /// </summary>
    /// <param name="id">ID of the origin to update</param>
    /// <param name="updateDto">New origin data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateOriginDto updateDto)
    {
        var success = await _originService.UpdateOriginAsync(id, updateDto);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes an origin by its ID
    /// </summary>
    /// <param name="id">ID of the origin to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _originService.DeleteOriginAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
