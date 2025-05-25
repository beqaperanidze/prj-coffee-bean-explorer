using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserListsController : ControllerBase
{
    private readonly IUserListService _userListService;

    public UserListsController(IUserListService userListService)
    {
        _userListService = userListService;
    }

    /// <summary>
    /// Gets all lists
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<UserListDto>> GetAll()
    {
        var lists = _userListService.GetAllLists();
        return Ok(lists);
    }

    /// <summary>
    /// Gets a specific list by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<UserListDto> GetById(int id)
    {
        var list = _userListService.GetListById(id);
        if (list is null) return NotFound();

        return Ok(list);
    }

    /// <summary>
    /// Gets all lists for a specific user
    /// </summary>
    [HttpGet("user/{userId:int}")]
    public ActionResult<IEnumerable<UserListDto>> GetUserLists(int userId)
    {
        var lists = _userListService.GetListsByUserId(userId);
        return Ok(lists);
    }

    /// <summary>
    /// Creates a new list for the current user
    /// </summary>
    [HttpPost]
    public ActionResult<UserListDto> Create([FromBody] CreateUserListDto dto, [FromQuery] int userId)
    {
        var list = _userListService.CreateList(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
    }

    /// <summary>
    /// Updates an existing list
    /// </summary>
    [HttpPut("{id:int}")]
    public ActionResult<UserListDto> Update(int id, [FromBody] UpdateUserListDto dto, [FromQuery] int userId)
    {
        var list = _userListService.UpdateList(id, dto, userId);
        if (list is null) return NotFound();
        return Ok(list);
    }

    /// <summary>
    /// Deletes a list
    /// </summary>
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id, [FromQuery] int userId)
    {
        var success = _userListService.DeleteList(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Gets all beans in a list
    /// </summary>
    [HttpGet("{id:int}/beans")]
    public ActionResult<IEnumerable<BeanDto>> GetBeansInList(int id)
    {
        var list = _userListService.GetListById(id);
        if (list is null) return NotFound();

        var beans = _userListService.GetBeansInList(id);
        return Ok(beans);
    }

    /// <summary>
    /// Adds a bean to a list
    /// </summary>
    [HttpPost("{listId:int}/beans/{beanId:int}")]
    public ActionResult AddBeanToList(int listId, int beanId, [FromQuery] int userId)
    {
        var success = _userListService.AddBeanToList(listId, beanId, userId);
        if (!success) return BadRequest();
        return NoContent();
    }

    /// <summary>
    /// Removes a bean from a list
    /// </summary>
    [HttpDelete("{listId:int}/beans/{beanId:int}")]
    public ActionResult RemoveBeanFromList(int listId, int beanId, [FromQuery] int userId)
    {
        var success = _userListService.RemoveBeanFromList(listId, beanId, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}
