using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Application.DTOs;

public class BeanDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OriginId { get; set; }
    public required string OriginCountry { get; set; }
    public string? OriginRegion { get; set; }
    public RoastLevel RoastLevel { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<TagDto>? Tags { get; set; }
}

public class CreateBeanDto
{
    public required string Name { get; set; }
    public int OriginId { get; set; }
    public RoastLevel RoastLevel { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<int>? TagIds { get; set; }
}

public class UpdateBeanDto
{
    public required string Name { get; set; }
    public int OriginId { get; set; }
    public RoastLevel RoastLevel { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<int>? TagIds { get; set; }
}