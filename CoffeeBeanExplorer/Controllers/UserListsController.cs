using System.Collections.Generic;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserListController : ControllerBase
{
    private readonly IUserListService _userListService;

    public UserListController(IUserListService userListService)
    {
        _userListService = userListService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetAll()
    {
        var lists = await _userListService.GetAllListsAsync();
        return Ok(lists);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserListDto>> GetById(int id)
    {
        var list = await _userListService.GetListByIdAsync(id);
        if (list is null) return NotFound();
        return Ok(list);
    }

    [HttpGet("users/{userId:int}")]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetUserLists(int userId)
    {
        var lists = await _userListService.GetListsByUserIdAsync(userId);
        return Ok(lists);
    }

    [HttpPost]
    public async Task<ActionResult<UserListDto>> Create([FromBody] CreateUserListDto dto, [FromQuery] int userId)
    {
        var list = await _userListService.CreateListAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserListDto>> Update(int id, [FromBody] UpdateUserListDto dto,
        [FromQuery] int userId)
    {
        var list = await _userListService.UpdateListAsync(id, dto, userId);
        if (list is null) return NotFound();
        return Ok(list);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, [FromQuery] int userId)
    {
        var success = await _userListService.DeleteListAsync(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("{listId:int}/beans/{beanId:int}")]
    public async Task<ActionResult> AddBeanToList(int listId, int beanId, [FromQuery] int userId)
    {
        var success = await _userListService.AddBeanToListAsync(listId, beanId, userId);
        if (!success) return BadRequest();
        return NoContent();
    }

    [HttpDelete("{listId:int}/beans/{beanId:int}")]
    public async Task<ActionResult> RemoveBeanFromList(int listId, int beanId, [FromQuery] int userId)
    {
        var success = await _userListService.RemoveBeanFromListAsync(listId, beanId, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}
