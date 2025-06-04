namespace CoffeeBeanExplorer.Application.DTOs;

public class OriginDto
{
    public int Id { get; set; }
    public required string Country { get; set; } 
    public string? Region { get; set; }
}

public class CreateOriginDto
{
    public required string Country { get; set; } 
    public string? Region { get; set; }
}

public class UpdateOriginDto
{
    public required string Country { get; set; } 
    public string? Region { get; set; }
}