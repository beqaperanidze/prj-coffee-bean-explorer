namespace CoffeeBeanExplorer.Domain.Models;

public class Origin
{
    public int Id { get; set; }
    public required string Country { get; set; }
    public string? Region { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Bean> Beans { get; set; } = new List<Bean>();
}