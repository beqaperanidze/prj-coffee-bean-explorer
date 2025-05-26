namespace CoffeeBeanExplorer.Domain.Models;

public class BeanTag
{
    public int BeanId { get; set; }
    public int TagId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Bean? Bean { get; set; }
    public Tag? Tag { get; set; }
}