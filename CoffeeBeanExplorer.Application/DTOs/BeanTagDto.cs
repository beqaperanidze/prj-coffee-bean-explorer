namespace CoffeeBeanExplorer.Application.DTOs;

public class BeanTagDto
{
    public int BeanId { get; set; }
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
}

public class CreateBeanTagDto
{
    public int BeanId { get; set; }
    public int TagId { get; set; }
}