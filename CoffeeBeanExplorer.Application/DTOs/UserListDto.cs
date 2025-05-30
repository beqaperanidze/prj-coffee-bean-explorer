namespace CoffeeBeanExplorer.Application.DTOs;

public class UserListDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int UserId { get; set; }
    public required ICollection<ListItemDto> Items { get; set; }
}

public class CreateUserListDto
{
    public required string Name { get; set; } 
}

public class UpdateUserListDto
{
    public required string Name { get; set; } 
}