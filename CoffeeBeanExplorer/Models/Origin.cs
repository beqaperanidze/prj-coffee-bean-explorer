using System.ComponentModel.DataAnnotations;

namespace CoffeeBeanExplorer.Models;

public class Origin
{
    public int Id { get; set; }

    [Required] [StringLength(100)] public string Country { get; set; } = string.Empty;

    [StringLength(100)] public string? Region { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdDateTime { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Bean> Beans { get; set; } = new List<Bean>();
}