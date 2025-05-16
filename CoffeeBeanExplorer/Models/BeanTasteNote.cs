using CoffeeBeanExplorer.Enums;

namespace CoffeeBeanExplorer.Models;

public class BeanTasteNote
{
    public int BeanId { get; set; }
    public TasteNote TasteNote { get; set; }

    public Bean Bean { get; set; } = null!;
}
