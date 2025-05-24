using System.ComponentModel.DataAnnotations;

namespace CoffeeBeanExplorer.Domain.Models;

public class Origin
{
    public int Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? Region { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}