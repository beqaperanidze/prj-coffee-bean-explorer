using System;

namespace CoffeeBeanExplorer.Domain.Models;

public class ListItem
{
    public int ListId { get; set; }
    public int BeanId { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserList? List { get; set; }
    public Bean? Bean { get; set; }
}