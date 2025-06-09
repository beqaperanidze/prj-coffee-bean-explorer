using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Enums;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class AuthService(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthService> logger)
    : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(UserRegistrationDto request)
    {
        logger.LogInformation("Registering user with username: {Username}", request.Username);
        var existenceCheck = await userRepository.CheckUserExistsAsync(request.Username, request.Email);

        if (existenceCheck.UsernameExists)
        {
            logger.LogWarning("Username already taken: {Username}", request.Username);
            throw new InvalidOperationException("Username is already taken");
        }

        if (existenceCheck.EmailExists)
        {
            logger.LogWarning("Email already registered: {Email}", request.Email);
            throw new InvalidOperationException("Email is already registered");
        }

        var salt = GenerateSalt();
        var passwordHash = HashPassword(request.Password, salt);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Salt = salt,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await userRepository.AddAsync(user);
        logger.LogInformation("User registered successfully: {Username}", request.Username);

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(UserLoginRequest request)
    {
        logger.LogInformation("User login attempt: {Username}", request.Username);
        var user = await userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            logger.LogWarning("Login failed - user not found: {Username}", request.Username);
            throw new InvalidOperationException("Invalid username or password");
        }

        if (string.IsNullOrEmpty(user.Salt))
        {
            logger.LogWarning("Login failed - invalid credentials for user: {Username}", request.Username);
            throw new InvalidOperationException("Invalid credentials");
        }

        if (!VerifyPassword(request.Password, user.PasswordHash, user.Salt))
        {
            logger.LogWarning("Login failed - password mismatch for user: {Username}", request.Username);
            throw new InvalidOperationException("Invalid username or password");
        }

        user.LastLogin = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
        logger.LogInformation("Login successful for user: {Username}", request.Username);

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
    {
        logger.LogInformation("Refreshing token");
        var principal = jwtService.GetPrincipalFromExpiredToken(token);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            logger.LogWarning("Token refresh failed - invalid token");
            throw new InvalidOperationException("Invalid token");
        }

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("Token refresh failed - user not found for ID: {UserId}", userId);
            throw new InvalidOperationException("Invalid token");
        }

        var storedRefreshToken = user.RefreshTokens.SingleOrDefault(r =>
            r.Token == refreshToken && r.IsActive);

        if (storedRefreshToken == null)
        {
            logger.LogWarning("Token refresh failed - invalid refresh token");
            throw new InvalidOperationException("Invalid refresh token");
        }

        await RevokeTokenAsync(refreshToken, user.Id, "Replaced by new token");
        logger.LogInformation("Token refreshed successfully");

        return await GenerateAuthResponse(user);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, int userId, string? reason = null)
    {
        logger.LogInformation("Revoking token for user ID: {UserId}", userId);
        var user = await userRepository.GetByIdAsync(userId);

        var storedToken = user?.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken);
        if (storedToken == null)
        {
            logger.LogWarning("Token revocation failed - token not found for user ID: {UserId}", userId);
            return false;
        }

        if (!storedToken.IsActive)
        {
            logger.LogWarning("Token revocation failed - token already inactive for user ID: {UserId}", userId);
            return false;
        }

        storedToken.Revoked = DateTime.UtcNow;
        storedToken.ReasonRevoked = reason ?? "Revoked without reason specified";

        logger.LogInformation("Token revoked successfully for user ID: {UserId}", userId);
        return user is not null && await userRepository.UpdateAsync(user);
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(User user)
    {
        var jwtToken = jwtService.GenerateJwtToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        refreshToken.UserId = user.Id;
        user.RefreshTokens.Add(refreshToken);
        await userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    private static string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private static string HashPassword(string password, string salt)
    {
        using var hmac = new HMACSHA512(Convert.FromBase64String(salt));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        try
        {
            if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var computedHashString = Convert.ToBase64String(computedHash);

            return string.Equals(computedHashString, storedHash, StringComparison.Ordinal);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Password verification failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}