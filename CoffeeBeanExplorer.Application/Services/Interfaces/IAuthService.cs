using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegistrationDto request);
    Task<AuthResponseDto> LoginAsync(UserLoginRequest request);
    Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken, int userId, string? reason = null);
}