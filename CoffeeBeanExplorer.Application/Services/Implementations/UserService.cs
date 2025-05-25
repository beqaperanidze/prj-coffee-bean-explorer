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

    public IEnumerable<UserDto> GetAllUsers()
    {
        return _repository.GetAll().Select(MapToDto);
    }

    public UserDto? GetUserById(int id)
    {
        var user = _repository.GetById(id);
        return user != null ? MapToDto(user) : null;
    }

    public UserDto RegisterUser(UserRegistrationDto dto)
    {
        if (_repository.GetByUsername(dto.Username) != null)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        if (_repository.GetByEmail(dto.Email) != null)
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

        var addedUser = _repository.Add(user);
        return MapToDto(addedUser);
    }

    public UserDto? UpdateUser(int id, UserUpdateDto dto)
    {
        var user = _repository.GetById(id);
        if (user is null) return null;

        if (dto.Username != null && user.Username != dto.Username)
        {
            if (_repository.GetByUsername(dto.Username) != null)
            {
                throw new InvalidOperationException("Username is already taken");
            }

            user.Username = dto.Username;
        }

        if (dto.Email != null && user.Email != dto.Email)
        {
            if (_repository.GetByEmail(dto.Email) != null)
            {
                throw new InvalidOperationException("Email is already registered");
            }

            user.Email = dto.Email;
        }

        if (dto.FirstName != null)
            user.FirstName = dto.FirstName;

        if (dto.LastName != null)
            user.LastName = dto.LastName;

        return _repository.Update(user) ? MapToDto(user) : null;
    }

    public bool DeleteUser(int id)
    {
        return _repository.Delete(id);
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