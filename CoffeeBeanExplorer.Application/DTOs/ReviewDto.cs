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
    [Required] public int BeanId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(500)] public string? Comment { get; set; }
}

public class UpdateReviewDto
{
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(500)] public string? Comment { get; set; }
}