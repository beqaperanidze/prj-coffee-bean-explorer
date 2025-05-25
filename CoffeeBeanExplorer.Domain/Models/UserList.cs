using System;
using System.Collections.Generic;

namespace CoffeeBeanExplorer.Domain.Models;

public class UserList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User? User { get; set; }
    public ICollection<ListItem> Items { get; set; } = new List<ListItem>();
}