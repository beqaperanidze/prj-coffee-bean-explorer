using System.ComponentModel.DataAnnotations;
using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Models.DTOs;

public class UserListDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int BeanId { get; set; }
    public string BeanName { get; set; } = string.Empty;
    public CollectionType Type { get; set; }
    public string CollectionTypeName => Type.ToString();
    public DateTime CreatedAt { get; set; }
    
}

public class CreateUserListDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required] 
    public int BeanId { get; set; }

    [Required] 
    public CollectionType Type { get; set; }
}

public class UpdateUserListDto
{
    [Required] public CollectionType Type { get; set; }
}