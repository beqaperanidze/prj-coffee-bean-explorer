using System.Text.Json.Serialization;
using CoffeeBeanExplorer.Domain.Enums;

namespace CoffeeBeanExplorer.Domain.Models;

public class UserList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BeanId { get; set; }
    public CollectionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore] public virtual User User { get; set; } = null!;
    [JsonIgnore] public virtual Bean Bean { get; set; } = null!;
}