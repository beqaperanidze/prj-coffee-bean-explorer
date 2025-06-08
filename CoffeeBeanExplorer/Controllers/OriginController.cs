using CoffeeBeanExplorer.Application.DTOs;
    using CoffeeBeanExplorer.Application.Origins.Commands;
    using CoffeeBeanExplorer.Application.Origins.Queries;
    using CoffeeBeanExplorer.Domain.Enums;
    using CoffeeBeanExplorer.Domain.Exceptions;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    
    namespace CoffeeBeanExplorer.Controllers;
    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/origins")]
    [Authorize]
    public class OriginController(IMediator mediator, ILogger<OriginController> logger) : ControllerBase
    {
        /// <summary>
        ///     Retrieves all coffee origins
        /// </summary>
        /// <returns>List of all origins</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OriginDto>>> GetAll()
        {
            logger.LogInformation("Retrieving all coffee origins");
            var origins = await mediator.Send(new GetAllOriginsQuery());
            logger.LogInformation("Successfully retrieved {OriginCount} origins", origins.Count());
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
            {
                logger.LogWarning("Invalid origin ID format: {OriginId}", id);
                throw new BadRequestException("Invalid ID format or value too large.");
            }
    
            logger.LogInformation("Retrieving origin with ID: {OriginId}", parsedId);
            var origin = await mediator.Send(new GetOriginByIdQuery(parsedId));
            if (origin is null)
            {
                logger.LogWarning("Origin not found for ID: {OriginId}", parsedId);
                throw new NotFoundException($"Origin with ID {parsedId} not found.");
            }
            
            logger.LogInformation("Origin retrieved successfully with ID: {OriginId}", parsedId);
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
            try
            {
                logger.LogInformation("Creating new coffee origin with Country: {Country}", createDto.Country);
                var origin = await mediator.Send(new CreateOriginCommand(createDto));
                logger.LogInformation("Created new coffee origin with ID: {OriginId}", origin.Id);
                return CreatedAtAction(nameof(GetById), new { id = origin.Id }, origin);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating origin");
                throw new InternalServerErrorException($"An error occurred while creating origin: {ex.Message}");
            }
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
            {
                logger.LogWarning("Invalid origin ID format: {OriginId}", id);
                throw new BadRequestException("Invalid ID format or value too large.");
            }
    
            logger.LogInformation("Attempting to update origin with ID: {OriginId}", parsedId);
            var success = await mediator.Send(new UpdateOriginCommand(parsedId, updateDto));
            if (!success)
            {
                logger.LogWarning("Origin not found for update, ID: {OriginId}", parsedId);
                throw new NotFoundException($"Origin with ID {parsedId} not found.");
            }
    
            logger.LogInformation("Origin updated successfully, ID: {OriginId}", parsedId);
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
            {
                logger.LogWarning("Invalid origin ID format for deletion: {OriginId}", id);
                throw new BadRequestException("Invalid ID format or value too large.");
            }
    
            logger.LogInformation("Attempting to delete origin with ID: {OriginId}", parsedId);
            var success = await mediator.Send(new DeleteOriginCommand(parsedId));
            if (!success)
            {
                logger.LogWarning("Origin not found for deletion, ID: {OriginId}", parsedId);
                throw new NotFoundException($"Origin with ID {parsedId} not found.");
            }
    
            logger.LogInformation("Origin deleted successfully, ID: {OriginId}", parsedId);
            return NoContent();
        }
    }
