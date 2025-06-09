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
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="request">User registration information</param>
    /// <returns>Authentication tokens for the new user</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto request)
    {
        try
        {
            var response = await authService.RegisterAsync(request);
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
            throw new BadRequestException(ex.Message);
        }
    }

    /// <summary>
    /// Authenticate an existing user.
    /// </summary>
    /// <param name="request">User login credentials</param>
    /// <returns>Authentication tokens for the user</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        try
        {
            var response = await authService.LoginAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            throw new UnauthorizedException(ex.Message);
        }
    }

    /// <summary>
    /// Generate new authentication tokens using a refresh token.
    /// </summary>
    /// <param name="request">Current token and refresh token</param>
    /// <returns>New authentication tokens</returns>
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
            throw new BadRequestException(ex.Message);
        }
        catch (Exception)
        {
            throw new BadRequestException("Invalid token");
        }
    }

    /// <summary>
    /// Invalidate a refresh token.
    /// </summary>
    /// <param name="request">The refresh token to revoke</param>
    /// <returns>Confirmation of token revocation</returns>
    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(RevokeTokenRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
            throw new UnauthorizedException("User is not authenticated or has invalid ID.");

        var success = await authService.RevokeTokenAsync(request.RefreshToken, userIdInt, "Revoked by user");
        if (!success) throw new NotFoundException("Token not found or already revoked");

        return Ok(new { message = "Token revoked" });
    }
}
