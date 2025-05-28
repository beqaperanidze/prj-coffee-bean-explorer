using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Application.DTOs;

public class BeanTagMapping
{
    public int BeanId { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}