using System.ComponentModel.DataAnnotations;
using CoffeeBeanExplorer.Enums;

namespace CoffeeBeanExplorer.Models;

public class Bean
{
    public int Id { get; set; }

    [Required] [StringLength(100)] public string Name { get; set; } = string.Empty;

    public int OriginId { get; set; }

    public RoastLevel RoastLevel { get; set; } = RoastLevel.Light;

    [StringLength(500)] public string Description { get; set; } = string.Empty;

    public decimal? Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required] public Origin Origin { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<UserList> UserCollections { get; set; } = new List<UserList>();
    public virtual ICollection<BeanTasteNote> BeanTasteNotes { get; set; } = new List<BeanTasteNote>();
}