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

    public IEnumerable<UserListDto> GetAllLists()
    {
        return _repository.GetAll().Select(MapToDto);
    }

    public UserListDto? GetListById(int id)
    {
        var list = _repository.GetById(id);
        return list != null ? MapToDto(list) : null;
    }

    public IEnumerable<UserListDto> GetListsByUserId(int userId)
    {
        return _repository.GetByUserId(userId).Select(MapToDto);
    }

    public UserListDto CreateList(CreateUserListDto dto, int userId)
    {
        var list = new UserList
        {
            Name = dto.Name,
            UserId = userId,
            Items = []
        };

        var addedList = _repository.Add(list);
        return MapToDto(addedList);
    }

    public UserListDto? UpdateList(int id, UpdateUserListDto dto, int userId)
    {
        var list = _repository.GetById(id);
        if (list is null || list.UserId != userId) return null;

        list.Name = dto.Name;

        return _repository.Update(list) ? MapToDto(list) : null;
    }

    public bool DeleteList(int id, int userId)
    {
        var list = _repository.GetById(id);
        if (list is null || list.UserId != userId) return false;

        return _repository.Delete(id);
    }

    public bool AddBeanToList(int listId, int beanId, int userId)
    {
        var list = _repository.GetById(listId);
        if (list is null || list.UserId != userId) return false;

        var bean = _beanRepository.GetById(beanId);
        return bean is not null && _repository.AddBeanToList(listId, beanId);
    }

    public bool RemoveBeanFromList(int listId, int beanId, int userId)
    {
        var list = _repository.GetById(listId);
        if (list is null || list.UserId != userId) return false;

        return _repository.RemoveBeanFromList(listId, beanId);
    }

    public IEnumerable<BeanDto> GetBeansInList(int listId)
    {
        return _repository.GetBeansInList(listId)
            .Select(b => new BeanDto
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