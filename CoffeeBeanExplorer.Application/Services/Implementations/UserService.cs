using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> RegisterUserAsync(UserRegistrationDto dto)
    {
        if (await _repository.GetByUsernameAsync(dto.Username) != null)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        if (await _repository.GetByEmailAsync(dto.Email) != null)
        {
            throw new InvalidOperationException("Email is already registered");
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        var addedUser = await _repository.AddAsync(user);
        return MapToDto(addedUser);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null) return null;

        if (dto.Username != null && user.Username != dto.Username)
        {
            if (await _repository.GetByUsernameAsync(dto.Username) != null)
            {
                throw new InvalidOperationException("Username is already taken");
            }

            user.Username = dto.Username;
        }

        if (dto.Email != null && user.Email != dto.Email)
        {
            if (await _repository.GetByEmailAsync(dto.Email) != null)
            {
                throw new InvalidOperationException("Email is already registered");
            }

            user.Email = dto.Email;
        }

        if (dto.FirstName != null)
            user.FirstName = dto.FirstName;

        if (dto.LastName != null)
            user.LastName = dto.LastName;

        return await _repository.UpdateAsync(user) ? MapToDto(user) : null;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Role = user.Role
        };
    }
}