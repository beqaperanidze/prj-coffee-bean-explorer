namespace CoffeeBeanExplorer.Models.DTOs;

public class OriginDto
{
    public int Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? Region { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdDateTime { get; set; }
}

public class CreateOriginDto
{
    public string Country { get; set; } = string.Empty;
    public string? Region { get; set; }
}

public class UpdateOriginDto
{
    public string Country { get; set; } = string.Empty;
    public string? Region { get; set; }
}