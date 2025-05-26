namespace CoffeeBeanExplorer.Application.DTOs;

public class UserListDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ListItemSummaryDto>? Items { get; set; } 
}

public class ListItemSummaryDto
{
    public int BeanId { get; set; }
    public string BeanName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
}

public class CreateUserListDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateUserListDto
{
    public string Name { get; set; } = string.Empty;
}