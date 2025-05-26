using System.ComponentModel.DataAnnotations;
using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public UserRole Role { get; set; }
}

public class UserRegistrationDto
{
    [Required] [MaxLength(50)] public string Username { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

    [Required] [MinLength(8)] public string Password { get; set; } = string.Empty;

    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)] public string LastName { get; set; } = string.Empty;
}

public class UserUpdateDto
{
    [MaxLength(50)] public string? Username { get; set; }

    [EmailAddress] public string? Email { get; set; }

    [MaxLength(100)] public string? FirstName { get; set; }

    [MaxLength(100)] public string? LastName { get; set; }
}