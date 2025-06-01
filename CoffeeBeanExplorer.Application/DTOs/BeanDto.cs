using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Application.DTOs;

public record BeanDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public int OriginId { get; init; }
    public required string OriginCountry { get; init; }
    public string? OriginRegion { get; init; }
    public RoastLevel RoastLevel { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public List<TagDto>? Tags { get; init; }
}

public record CreateBeanDto
{
    public required string Name { get; init; }
    public required int OriginId { get; init; }
    public required RoastLevel RoastLevel { get; init; }
    public string? Description { get; init; }
    public required decimal Price { get; init; }
    public List<int>? TagIds { get; init; }
}

public record UpdateBeanDto
{
    public required string Name { get; init; }
    public required int OriginId { get; init; }
    public required RoastLevel RoastLevel { get; init; }
    public string? Description { get; init; }
    public required decimal Price { get; init; }
    public List<int>? TagIds { get; init; }
}
