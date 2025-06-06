namespace CoffeeBeanExplorer.Domain.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; } 
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Revoked { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
