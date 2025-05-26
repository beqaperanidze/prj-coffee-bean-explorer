namespace CoffeeBeanExplorer.Application.DTOs;

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateTagDto
{
    public string Name { get; set; } = string.Empty;
}