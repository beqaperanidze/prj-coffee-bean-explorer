namespace CoffeeBeanExplorer.Application.DTOs;

public class BeanTagDto
{
    public int BeanId { get; set; }
    public int TagId { get; set; }
    public required string TagName { get; set; } 
}

public class CreateBeanTagDto
{
    public int BeanId { get; set; }
    public int TagId { get; set; }
}