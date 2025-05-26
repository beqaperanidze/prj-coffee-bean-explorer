using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } 
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<UserList> Lists { get; set; } = new List<UserList>();
}