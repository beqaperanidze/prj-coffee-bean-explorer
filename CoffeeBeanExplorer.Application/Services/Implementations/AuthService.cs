using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Enums;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using Serilog;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class AuthService(IUserRepository userRepository, IJwtService jwtService) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(UserRegistrationDto request)
    {
        var existenceCheck = await userRepository.CheckUserExistsAsync(request.Username, request.Email);

        if (existenceCheck.UsernameExists)
            throw new InvalidOperationException("Username is already taken");

        if (existenceCheck.EmailExists)
            throw new InvalidOperationException("Email is already registered");

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

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password");

        if (string.IsNullOrEmpty(user.Salt))
            throw new InvalidOperationException("Invalid credentials");

        if (!VerifyPassword(request.Password, user.PasswordHash, user.Salt))
            throw new InvalidOperationException("Invalid username or password");

        user.LastLogin = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(token);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new InvalidOperationException("Invalid token");

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Invalid token");

        var storedRefreshToken = user.RefreshTokens.SingleOrDefault(r =>
            r.Token == refreshToken && r.IsActive);

        if (storedRefreshToken == null)
            throw new InvalidOperationException("Invalid refresh token");

        await RevokeTokenAsync(refreshToken, user.Id, "Replaced by new token");

        return await GenerateAuthResponse(user);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, int userId, string? reason = null)
    {
        var user = await userRepository.GetByIdAsync(userId);

        var storedToken = user?.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken);
        if (storedToken == null)
            return false;

        if (!storedToken.IsActive)
            return false;

        storedToken.Revoked = DateTime.UtcNow;
        storedToken.ReasonRevoked = reason ?? "Revoked without reason specified";

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

    private static bool VerifyPassword(string password, string storedHash, string storedSalt)
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
            Log.Error(ex, "Password verification failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}
