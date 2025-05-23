﻿using System.ComponentModel.DataAnnotations;

namespace CoffeeBeanExplorer.Models;

public class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BeanId { get; set; }

    [Range(1, 5, ErrorMessage = "Must be  1-5")]
    public int Rating { get; set; }

    [StringLength(500)] public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required] public User User { get; set; } = null!;

    [Required] public Bean Bean { get; set; } = null!;
}