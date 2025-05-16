using CoffeeBeanExplorer.Enums;

namespace CoffeeBeanExplorer.Models;

public class UserList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BeanId { get; set; }
    public CollectionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual Bean Bean { get; set; } = null!;
}