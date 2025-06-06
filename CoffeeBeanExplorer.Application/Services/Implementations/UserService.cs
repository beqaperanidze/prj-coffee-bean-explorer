using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class UserService(IUserRepository repository, IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);
        return user is not null ? mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto dto)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) return null;

        if (dto.Username is not null && user.Username != dto.Username)
        {
            if (await repository.GetByUsernameAsync(dto.Username) is not null)
                throw new InvalidOperationException("Username is already taken");

            user.Username = dto.Username;
        }

        if (dto.Email is not null && user.Email != dto.Email)
        {
            if (await repository.GetByEmailAsync(dto.Email) is not null)
                throw new InvalidOperationException("Email is already registered");

            user.Email = dto.Email;
        }

        if (dto.FirstName is not null)
            user.FirstName = dto.FirstName;

        if (dto.LastName is not null)
            user.LastName = dto.LastName;

        return await repository.UpdateAsync(user) ? mapper.Map<UserDto>(user) : null;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }
}
