using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Domain.Models;

public class User
{
    public int Id { get; set; }

    [Required] [MaxLength(50)] public string Username { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

    [Required] public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)] public string LastName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserRole Role { get; set; } = UserRole.User;
    [JsonIgnore] public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    [JsonIgnore] public virtual ICollection<UserList> UserCollections { get; set; } = new List<UserList>();
}