using System.ComponentModel.DataAnnotations;

namespace CoffeeBeanExplorer.Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Username { get; set; }
    public int BeanId { get; set; }
    public required string BeanName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class CreateReviewDto
{
    public int BeanId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class UpdateReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
