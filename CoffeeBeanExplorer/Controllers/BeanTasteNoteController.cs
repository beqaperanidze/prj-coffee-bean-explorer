using CoffeeBeanExplorer.Domain.Enums;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Models;
using CoffeeBeanExplorer.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BeanTasteNoteController : ControllerBase
{
    private static readonly List<BeanTasteNote> TasteNotes = [];

    /// <summary>
    /// Retrieves all bean taste notes
    /// </summary>
    /// <returns>List of all taste notes</returns>
    [HttpGet]
    public ActionResult<IEnumerable<BeanTasteNoteDto>> GetAll()
    {
        var tasteDtos = TasteNotes.Select(MapToDto).ToList();
        return Ok(tasteDtos);
    }

    /// <summary>
    /// Retrieves taste notes for a specific bean
    /// </summary>
    /// <param name="beanId">The ID of the bean</param>
    /// <returns>List of taste notes for the bean</returns>
    [HttpGet("byBean/{beanId:int}")]
    public ActionResult<IEnumerable<BeanTasteNoteDto>> GetByBeanId(int beanId)
    {
        var beanTasteNotes = TasteNotes.Where(t => t.BeanId == beanId).Select(MapToDto).ToList();
        return Ok(beanTasteNotes);
    }

    /// <summary>
    /// Retrieves beans by taste note
    /// </summary>
    /// <param name="tasteNote">The taste note to filter by</param>
    /// <returns>List of beans with the specified taste note</returns>
    [HttpGet("byTaste/{tasteNote}")]
    public ActionResult<IEnumerable<BeanTasteNoteDto>> GetByTasteNote(TasteNote tasteNote)
    {
        var beanTasteNotes = TasteNotes.Where(t => t.TasteNote == tasteNote).Select(MapToDto).ToList();
        return Ok(beanTasteNotes);
    }

    /// <summary>
    /// Creates a new bean taste note association
    /// </summary>
    /// <param name="createDto">The taste note data to create</param>
    /// <returns>The created taste note</returns>
    [HttpPost]
    public ActionResult<BeanTasteNoteDto> Create(CreateBeanTasteNoteDto createDto)
    {
        if (TasteNotes.Any(t => t.BeanId == createDto.BeanId && t.TasteNote == createDto.TasteNote))
        {
            return BadRequest(new { Message = "This taste note is already associated with the bean" });
        }

        var beanTasteNote = new BeanTasteNote
        {
            BeanId = createDto.BeanId,
            TasteNote = createDto.TasteNote,
            Bean = new Bean { Id = createDto.BeanId, Name = "Bean Name" }
        };

        TasteNotes.Add(beanTasteNote);
        return CreatedAtAction(nameof(GetByBeanId), new { beanId = beanTasteNote.BeanId }, MapToDto(beanTasteNote));
    }

    /// <summary>
    /// Deletes a taste note association from a bean
    /// </summary>
    /// <param name="beanId">Bean ID</param>
    /// <param name="tasteNote">Taste note to remove</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{beanId:int}/{tasteNote}")]
    public IActionResult Delete(int beanId, TasteNote tasteNote)
    {
        var beanTasteNote = TasteNotes.FirstOrDefault(t => t.BeanId == beanId && t.TasteNote == tasteNote);
        if (beanTasteNote == null) return NotFound();

        TasteNotes.Remove(beanTasteNote);
        return NoContent();
    }

    private static BeanTasteNoteDto MapToDto(BeanTasteNote tasteNote)
    {
        return new BeanTasteNoteDto
        {
            BeanId = tasteNote.BeanId,
            BeanName = tasteNote.Bean.Name,
            TasteNote = tasteNote.TasteNote
        };
    }
}
