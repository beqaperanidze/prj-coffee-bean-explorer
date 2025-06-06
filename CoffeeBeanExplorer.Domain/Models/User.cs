using CoffeeBeanExplorer.Domain.Enums;
using System.Collections.Generic;

namespace CoffeeBeanExplorer.Domain.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Salt { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<UserList> Lists { get; set; } = new List<UserList>();
}
