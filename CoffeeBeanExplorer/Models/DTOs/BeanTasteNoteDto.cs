using System.ComponentModel.DataAnnotations;
using CoffeeBeanExplorer.Enums;

namespace CoffeeBeanExplorer.Models.DTOs;

public class BeanTasteNoteDto
{
    public int BeanId { get; set; }
    public string BeanName { get; set; } = string.Empty;
    public TasteNote TasteNote { get; set; }
    public string TasteNoteName => TasteNote.ToString();
}

public class CreateBeanTasteNoteDto
{
    [Required] public int BeanId { get; set; }

    [Required] public TasteNote TasteNote { get; set; }
}