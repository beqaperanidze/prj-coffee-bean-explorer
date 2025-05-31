namespace CoffeeBeanExplorer.Application.DTOs;

public class ListItemDto
{
    public int ListId { get; set; }
    public int BeanId { get; set; }
    public required string BeanName { get; set; }
    public required string OriginCountry { get; set; }
    public string? OriginRegion { get; set; }
    public decimal Price { get; set; }
}

public class CreateListItemDto
{
    public int ListId { get; set; }
    public int BeanId { get; set; }
}