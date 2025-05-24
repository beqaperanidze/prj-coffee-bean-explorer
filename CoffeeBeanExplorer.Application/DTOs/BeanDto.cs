using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Application.DTOs;

public class BeanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OriginId { get; set; }
    public string OriginCountry { get; set; } = string.Empty;
    public string? OriginRegion { get; set; } = string.Empty;
    public RoastLevel RoastLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBeanDto
{
    public string Name { get; set; } = string.Empty;
    public int OriginId { get; set; }
    public RoastLevel RoastLevel { get; set; } = RoastLevel.Light;
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}

public class UpdateBeanDto
{
    public string Name { get; set; } = string.Empty;
    public int OriginId { get; set; }
    public RoastLevel RoastLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}