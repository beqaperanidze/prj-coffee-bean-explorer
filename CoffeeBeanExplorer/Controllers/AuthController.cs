using System.Security.Claims;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto request)
    {
        try
        {
            logger.LogInformation("Attempting to register user with username: {Username}", request.Username);
            var response = await authService.RegisterAsync(request);
            logger.LogInformation("User registered successfully with username: {Username}", request.Username);
            return Ok(new
            {
                message = "Registration successful",
                response.Token,
                response.RefreshToken,
                response.ExpiresAt
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Registration failed for username: {Username}", request.Username);
            throw new BadRequestException(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        try
        {
            logger.LogInformation("Attempting login for user with username: {Username}", request.Username);
            var response = await authService.LoginAsync(request);
            logger.LogInformation("Login successful for user with username: {Username}", request.Username);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Login failed for username: {Username}", request.Username);
            throw new UnauthorizedException(ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(request.Token, request.RefreshToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Token refresh failed");
            throw new BadRequestException(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Invalid token");
            throw new BadRequestException("Invalid token");
        }
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(RevokeTokenRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
        {
            logger.LogWarning("User authentication failed or invalid ID when revoking token");
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");
        }

        logger.LogInformation("Attempting to revoke token for user ID: {UserId}", userIdInt);
        var success = await authService.RevokeTokenAsync(request.RefreshToken, userIdInt, "Revoked by user");
        if (!success)
        {
            logger.LogWarning("Token not found or already revoked for user ID: {UserId}", userIdInt);
            throw new NotFoundException("Token not found or already revoked");
        }

        logger.LogInformation("Token successfully revoked for user ID: {UserId}", userIdInt);
        return Ok(new { message = "Token revoked" });
    }
}
