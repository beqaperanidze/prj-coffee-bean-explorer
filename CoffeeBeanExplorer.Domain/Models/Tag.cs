namespace CoffeeBeanExplorer.Domain.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<BeanTag> BeanTags { get; set; } = new List<BeanTag>();
}