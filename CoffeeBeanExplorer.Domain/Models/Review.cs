using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeBeanExplorer.Domain.Models;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BeanId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}