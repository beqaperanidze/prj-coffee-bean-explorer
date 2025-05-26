using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> RegisterUserAsync(UserRegistrationDto dto);
    Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto dto);
    Task<bool> DeleteUserAsync(int id);
}