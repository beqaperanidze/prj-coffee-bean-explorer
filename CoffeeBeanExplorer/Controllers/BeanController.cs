using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Enums;
using CoffeeBeanExplorer.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/beans")]
[Authorize]
public class BeanController(IBeanService beanService, ILogger<BeanController> logger) : ControllerBase
{
    /// <summary>
    ///     Get all coffee beans.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BeanDto>>> GetAll()
    {
        logger.LogInformation("Retrieving all coffee beans");
        var beans = await beanService.GetAllBeansAsync();
        logger.LogInformation("Successfully retrieved {BeanCount} coffee beans", beans.Count());
        return Ok(beans);
    }

    /// <summary>
    ///     Get a bean by its ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BeanDto>> GetById(string id)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid bean ID format: {BeanId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Retrieving bean with ID: {BeanId}", parsedId);
        var bean = await beanService.GetBeanByIdAsync(parsedId);
        if (bean == null)
        {
            logger.LogWarning("Bean not found for ID: {BeanId}", parsedId);
            throw new NotFoundException($"Bean with ID {parsedId} not found.");
        }

        logger.LogInformation("Bean retrieved successfully with ID: {BeanId}", parsedId);
        return Ok(bean);
    }

    /// <summary>
    ///     Create a new coffee bean.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<BeanDto>> Create(CreateBeanDto dto)
    {
        try
        {
            logger.LogInformation("Creating new coffee bean {@BeanData}",
                new { dto.Name, dto.OriginId, dto.Description });
            var bean = await beanService.CreateBeanAsync(dto);
            logger.LogInformation("Created new coffee bean with ID: {BeanId}", bean.Id);
            return CreatedAtAction(nameof(GetById), new { id = bean.Id }, bean);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Error occurred while creating bean");
            throw new InternalServerErrorException($"An error occurred while creating bean: {ex.Message}");
        }
    }

    /// <summary>
    ///     Update an existing bean.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Update(string id, UpdateBeanDto dto)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid bean ID format for update: {BeanId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Updating bean with ID: {BeanId}", parsedId);
        var success = await beanService.UpdateBeanAsync(parsedId, dto);
        if (!success)
        {
            logger.LogWarning("Bean not found for update, ID: {BeanId}", parsedId);
            throw new NotFoundException($"Bean with ID {parsedId} not found.");
        }

        logger.LogInformation("Bean {BeanId} updated successfully", parsedId);
        return NoContent();
    }

    /// <summary>
    ///     Delete a bean by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(UserRole.Brewer)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!int.TryParse(id, out var parsedId))
        {
            logger.LogWarning("Invalid bean ID format for deletion: {BeanId}", id);
            throw new BadRequestException("Invalid ID format or value too large.");
        }

        logger.LogInformation("Deleting bean with ID: {BeanId}", parsedId);
        var success = await beanService.DeleteBeanAsync(parsedId);
        if (!success)
        {
            logger.LogWarning("Bean not found for deletion, ID: {BeanId}", parsedId);
            throw new NotFoundException($"Bean with ID {parsedId} not found.");
        }

        logger.LogInformation("Bean {BeanId} deleted successfully", parsedId);
        return NoContent();
    }
}
