using System.Text.Json.Serialization;
using CoffeeBeanExplorer.Enums;

namespace CoffeeBeanExplorer.Models;

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