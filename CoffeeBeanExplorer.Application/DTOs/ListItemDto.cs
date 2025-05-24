namespace CoffeeBeanExplorer.Application.DTOs;

public class ListItemDto
{
    public int ListId { get; set; }
    public int BeanId { get; set; }
    public string BeanName { get; set; } = string.Empty; 
    public string OriginCountry { get; set; } = string.Empty;
    public string? OriginRegion { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateListItemDto
{
    public int ListId { get; set; }
    public int BeanId { get; set; }
}