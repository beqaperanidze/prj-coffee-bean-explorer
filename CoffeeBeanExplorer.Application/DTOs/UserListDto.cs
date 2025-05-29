namespace CoffeeBeanExplorer.Application.DTOs;

public class UserListDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ListItemDto> Items { get; set; }
}

public class CreateUserListDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateUserListDto
{
    public string Name { get; set; } = string.Empty;
}