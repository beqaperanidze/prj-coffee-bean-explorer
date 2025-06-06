using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class UserListService(IUserListRepository repository, IBeanRepository beanRepository, IMapper mapper)
    : IUserListService
{
    public async Task<IEnumerable<UserListDto>> GetAllListsAsync()
    {
        var lists = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<UserListDto>>(lists);
    }

    public async Task<UserListDto?> GetListByIdAsync(int id)
    {
        var list = await repository.GetByIdAsync(id);
        return list is not null ? mapper.Map<UserListDto>(list) : null;
    }

    public async Task<IEnumerable<UserListDto>> GetListsByUserIdAsync(int userId)
    {
        var lists = await repository.GetByUserIdAsync(userId);
        return mapper.Map<IEnumerable<UserListDto>>(lists);
    }

    public async Task<UserListDto> CreateListAsync(CreateUserListDto dto, int userId)
    {
        var list = new UserList
        {
            Name = dto.Name,
            UserId = userId,
            Items = []
        };

        var addedList = await repository.AddAsync(list);
        return mapper.Map<UserListDto>(addedList);
    }

    public async Task<UserListDto?> UpdateListAsync(int id, UpdateUserListDto dto, int userId)
    {
        var list = await repository.GetByIdAsync(id);
        if (list is null || list.UserId != userId) return null;

        list.Name = dto.Name;

        return await repository.UpdateAsync(list) ? mapper.Map<UserListDto>(list) : null;
    }

    public async Task<bool> DeleteListAsync(int id, int userId)
    {
        var list = await repository.GetByIdAsync(id);
        if (list is null || list.UserId != userId) return false;

        return await repository.DeleteAsync(id);
    }

    public async Task<bool> AddBeanToListAsync(int listId, int beanId, int userId)
    {
        var list = await repository.GetByIdAsync(listId);
        if (list is null || list.UserId != userId) return false;

        var bean = await beanRepository.GetByIdAsync(beanId);
        return bean is not null && await repository.AddBeanToListAsync(listId, beanId);
    }

    public async Task<bool> RemoveBeanFromListAsync(int listId, int beanId, int userId)
    {
        var list = await repository.GetByIdAsync(listId);
        if (list is null || list.UserId != userId) return false;

        return await repository.RemoveBeanFromListAsync(listId, beanId);
    }
}
