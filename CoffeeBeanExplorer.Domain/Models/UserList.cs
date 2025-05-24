using System.Text.Json.Serialization;
using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Domain.Models;

public class UserList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}