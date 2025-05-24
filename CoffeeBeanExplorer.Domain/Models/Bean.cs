using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Domain.Models;

public class Bean
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OriginId { get; set; }
    public RoastLevel RoastLevel { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}