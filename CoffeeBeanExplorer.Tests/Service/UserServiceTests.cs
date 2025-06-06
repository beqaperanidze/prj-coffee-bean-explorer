using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Implementations;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using Moq;
using Xunit;

namespace CoffeeBeanExplorer.Tests.Service;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedUsers()
    {
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Username = "user1",
                Email = "user1@mail.com",
                PasswordHash = "hash",
                Salt = "salt"
            }
        };
        var userDtos = new List<UserDto>
        {
            new()
            {
                Id = 1,
                Username = "user1",
                Email = "user1@mail.com",
                FirstName = "Test",
                LastName = "User"
            }
        };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        var service = new UserService(repoMock.Object, mapperMock.Object);

        var result = await service.GetAllUsersAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("user1", ((List<UserDto>)result)[0].Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserExists_ReturnsMappedUser()
    {
        var user = new User
        {
            Id = 1,
            Username = "user1",
            Email = "user1@mail.com",
            PasswordHash = "hash",
            Salt = "salt"
        };
        var userDto = new UserDto
        {
            Id = 1,
            Username = "user1",
            Email = "user1@mail.com",
            FirstName = "Test",
            LastName = "User"
        };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        var service = new UserService(repoMock.Object, mapperMock.Object);

        var result = await service.GetUserByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("user1", result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserNotFound_ReturnsNull()
    {
        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User)null!);

        var mapperMock = new Mock<IMapper>();

        var service = new UserService(repoMock.Object, mapperMock.Object);

        var result = await service.GetUserByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserAsync_ValidUpdate_ReturnsUpdatedUserDto()
    {
        const int userId = 1;
        var existingUser = new User
        {
            Id = userId,
            Username = "olduser",
            Email = "old@mail.com",
            PasswordHash = "hash",
            Salt = "salt"
        };
        var updateDto = new UserUpdateDto { Username = "newuser", Email = "new@mail.com" };
        var updatedUserDto = new UserDto
        {
            Id = userId,
            Username = "newuser",
            Email = "new@mail.com",
            FirstName = "Test",
            LastName = "User"
        };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        repoMock.Setup(r => r.GetByUsernameAsync("newuser")).ReturnsAsync((User)null!);
        repoMock.Setup(r => r.GetByEmailAsync("new@mail.com")).ReturnsAsync((User)null!);
        repoMock.Setup(r => r.UpdateAsync(existingUser)).ReturnsAsync(true);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(updatedUserDto);
        mapperMock.Setup(m => m.Map(updateDto, existingUser));

        var service = new UserService(repoMock.Object, mapperMock.Object);

        var result = await service.UpdateUserAsync(userId, updateDto);

        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("new@mail.com", result.Email);
        repoMock.Verify(r => r.UpdateAsync(existingUser), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_UserNotFound_ReturnsNull()
    {
        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User)null!);

        var mapperMock = new Mock<IMapper>();
        var service = new UserService(repoMock.Object, mapperMock.Object);

        var result = await service.UpdateUserAsync(1, new UserUpdateDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserAsync_UsernameTaken_ThrowsException()
    {
        const int userId = 1;
        var existingUser = new User
        {
            Id = userId,
            Username = "olduser",
            Email = "old@mail.com",
            PasswordHash = "hash",
            Salt = "salt"
        };
        var updateDto = new UserUpdateDto { Username = "takenuser" };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        repoMock.Setup(r => r.GetByUsernameAsync("takenuser")).ReturnsAsync(new User
        {
            Id = 2,
            Username = "takenuser",
            Email = "taken@mail.com",
            PasswordHash = "hash2",
            Salt = "salt2"
        });

        var mapperMock = new Mock<IMapper>();
        var service = new UserService(repoMock.Object, mapperMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateUserAsync(userId, updateDto));
    }

    [Fact]
    public async Task UpdateUserAsync_EmailTaken_ThrowsException()
    {
        const int userId = 1;
        var existingUser = new User
        {
            Id = userId,
            Username = "olduser",
            Email = "old@mail.com",
            PasswordHash = "hash",
            Salt = "salt"
        };
        var updateDto = new UserUpdateDto { Email = "taken@mail.com" };

        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        repoMock.Setup(r => r.GetByEmailAsync("taken@mail.com")).ReturnsAsync(new User
        {
            Id = 3,
            Username = "otheruser",
            Email = "taken@mail.com",
            PasswordHash = "hash3",
            Salt = "salt3"
        });

        var mapperMock = new Mock<IMapper>();
        var service = new UserService(repoMock.Object, mapperMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateUserAsync(userId, updateDto));
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsResult()
    {
        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var mapperMock = new Mock<IMapper>();
        var service = new UserService(repoMock.Object, mapperMock.Object);

        var result = await service.DeleteUserAsync(1);

        Assert.True(result);
    }
}
