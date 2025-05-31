namespace CoffeeBeanExplorer.Application.DTOs;

public class TagDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class CreateTagDto
{
    public required string Name { get; set; } 
}

public class UpdateTagDto
{
    public required string Name { get; set; } 
}