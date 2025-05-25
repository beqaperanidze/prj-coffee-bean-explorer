using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IUserService
{
    IEnumerable<UserDto> GetAllUsers();
    UserDto? GetUserById(int id);
    UserDto RegisterUser(UserRegistrationDto dto);
    UserDto? UpdateUser(int id, UserUpdateDto dto);
    bool DeleteUser(int id);
}