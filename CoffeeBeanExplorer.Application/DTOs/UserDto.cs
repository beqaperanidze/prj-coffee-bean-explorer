using System.ComponentModel.DataAnnotations;
using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; } 
    public required string Email { get; set; }
    public required string FirstName { get; set; } 
    public required string LastName { get; set; }
    public UserRole Role { get; set; }
}

public class UserRegistrationDto
{
    [Required] [MaxLength(50)] public required string Username { get; set; }

    [Required] [EmailAddress] public required string Email { get; set; }

    [Required] [MinLength(8)] public required string Password { get; set; }

    [MaxLength(100)] public required string FirstName { get; set; }

    [MaxLength(100)] public required string LastName { get; set; }
}

public class UserUpdateDto
{
    [MaxLength(50)] public string? Username { get; set; }

    [EmailAddress] public string? Email { get; set; }

    [MaxLength(100)] public string? FirstName { get; set; }

    [MaxLength(100)] public string? LastName { get; set; }
}