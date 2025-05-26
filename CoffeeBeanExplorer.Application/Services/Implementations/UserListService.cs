using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;

namespace CoffeeBeanExplorer.Application.Services.Implementations;

public class UserListService : IUserListService
{
    private readonly IUserListRepository _repository;
    private readonly IBeanRepository _beanRepository;

    public UserListService(IUserListRepository repository, IBeanRepository beanRepository)
    {
        _repository = repository;
        _beanRepository = beanRepository;
    }

    public async Task<IEnumerable<UserListDto>> GetAllListsAsync()
    {
        var lists = await _repository.GetAllAsync();
        return lists.Select(MapToDto);
    }

    public async Task<UserListDto?> GetListByIdAsync(int id)
    {
        var list = await _repository.GetByIdAsync(id);
        return list != null ? MapToDto(list) : null;
    }

    public async Task<IEnumerable<UserListDto>> GetListsByUserIdAsync(int userId)
    {
        var lists = await _repository.GetByUserIdAsync(userId);
        return lists.Select(MapToDto);
    }

    public async Task<UserListDto> CreateListAsync(CreateUserListDto dto, int userId)
    {
        var list = new UserList
        {
            Name = dto.Name,
            UserId = userId,
            Items = []
        };

        var addedList = await _repository.AddAsync(list);
        return MapToDto(addedList);
    }

    public async Task<UserListDto?> UpdateListAsync(int id, UpdateUserListDto dto, int userId)
    {
        var list = await _repository.GetByIdAsync(id);
        if (list is null || list.UserId != userId) return null;

        list.Name = dto.Name;

        return await _repository.UpdateAsync(list) ? MapToDto(list) : null;
    }

    public async Task<bool> DeleteListAsync(int id, int userId)
    {
        var list = await _repository.GetByIdAsync(id);
        if (list is null || list.UserId != userId) return false;

        return await _repository.DeleteAsync(id);
    }

    public async Task<bool> AddBeanToListAsync(int listId, int beanId, int userId)
    {
        var list = await _repository.GetByIdAsync(listId);
        if (list is null || list.UserId != userId) return false;

        var bean = await _beanRepository.GetByIdAsync(beanId);
        return bean is not null && await _repository.AddBeanToListAsync(listId, beanId);
    }

    public async Task<bool> RemoveBeanFromListAsync(int listId, int beanId, int userId)
    {
        var list = await _repository.GetByIdAsync(listId);
        if (list is null || list.UserId != userId) return false;

        return await _repository.RemoveBeanFromListAsync(listId, beanId);
    }

    public async Task<IEnumerable<BeanDto>> GetBeansInListAsync(int listId)
    {
        var beans = await _repository.GetBeansInListAsync(listId);
        return beans.Select(b => new BeanDto
        {
            Id = b.Id,
            Name = b.Name ?? string.Empty
        });
    }

    private static UserListDto MapToDto(UserList list)
    {
        return new UserListDto
        {
            Id = list.Id,
            Name = list.Name,
            UserId = list.UserId,
            CreatedAt = list.CreatedAt,
            UpdatedAt = list.UpdatedAt,
            BeansCount = list.Items?.Count ?? 0
        };
    }
}