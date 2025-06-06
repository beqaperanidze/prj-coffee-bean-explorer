using System.Security.Claims;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    RefreshToken GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
