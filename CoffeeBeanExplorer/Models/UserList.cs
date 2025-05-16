using CoffeeBeanExplorer.Enums;

namespace CoffeeBeanExplorer.Models;

public class UserList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid BeanId { get; set; }
    public CollectionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User? User { get; set; }
    public virtual Bean? Bean { get; set; }
}