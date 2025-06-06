using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Origins.Commands;
using CoffeeBeanExplorer.Application.Origins.Queries;
using CoffeeBeanExplorer.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/origins")]
[Authorize]
public class OriginController(IMediator mediator) : ControllerBase
{
    /// <summary>
    ///     Retrieves all coffee origins
    /// </summary>
    /// <returns>List of all origins</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OriginDto>>> GetAll()
    {
        var origins = await mediator.Send(new GetAllOriginsQuery());
        return Ok(origins);
    }

    /// <summary>
    ///     Retrieves a specific origin by its ID
    /// </summary>
    /// <param name="id">The ID of the origin to retrieve</param>
    /// <returns>The requested origin or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OriginDto>> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var origin = await mediator.Send(new GetOriginByIdQuery(parsedId));
        if (origin is null) return NotFound();
        return Ok(origin);
    }

    /// <summary>
    ///     Creates a new coffee origin
    /// </summary>
    /// <param name="createDto">The origin data to create</param>
    /// <returns>The created origin with its new ID</returns>
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<OriginDto>> Create([FromBody] CreateOriginDto createDto)
    {
        var origin = await mediator.Send(new CreateOriginCommand(createDto));
        return CreatedAtAction(nameof(GetById), new { id = origin.Id }, origin);
    }

    /// <summary>
    ///     Updates an existing origin by ID
    /// </summary>
    /// <param name="id">ID of the origin to update</param>
    /// <param name="updateDto">New origin data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Update(string id, UpdateOriginDto updateDto)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var success = await mediator.Send(new UpdateOriginCommand(parsedId, updateDto));
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Deletes an origin by its ID
    /// </summary>
    /// <param name="id">ID of the origin to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
            return BadRequest("Invalid ID format or value too large.");

        var success = await mediator.Send(new DeleteOriginCommand(parsedId));
        if (!success) return NotFound();
        return NoContent();
    }
}
