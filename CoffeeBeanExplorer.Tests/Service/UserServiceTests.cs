using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Services.Implementations;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace CoffeeBeanExplorer.Tests.Service;

public class UserServiceTests
{
    private readonly Faker _faker = new();
    private readonly IFixture _fixture;

    public UserServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedUsers()
    {
        var users = _fixture.CreateMany<User>(3).ToList();
        var userDtos = _fixture.CreateMany<UserDto>(3).ToList();

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var mapperMock = _fixture.Freeze<Mock<IMapper>>();
        mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        var service = _fixture.Create<UserService>();
        var result = await service.GetAllUsersAsync();

        var enumerable = result as UserDto[] ?? result.ToArray();
        enumerable.ShouldNotBeNull();
        enumerable.ShouldBe(userDtos);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserExists_ReturnsMappedUser()
    {
        var userId = _faker.Random.Int(1, 1000);
        var user = _fixture.Create<User>();
        var userDto = _fixture.Create<UserDto>();

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var mapperMock = _fixture.Freeze<Mock<IMapper>>();
        mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        var service = _fixture.Create<UserService>();
        var result = await service.GetUserByIdAsync(userId);

        result.ShouldNotBeNull();
        result.ShouldBe(userDto);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserNotFound_ReturnsNull()
    {
        var userId = _faker.Random.Int(1, 1000);

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null!);

        var service = _fixture.Create<UserService>();
        var result = await service.GetUserByIdAsync(userId);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateUserAsync_ValidUpdate_ReturnsUpdatedUserDto()
    {
        var userId = _faker.Random.Int(1, 1000);
        var existingUser = _fixture.Create<User>();

        var updateDto = _fixture.Build<UserUpdateDto>()
            .With(u => u.Username, _faker.Internet.UserName())
            .With(u => u.Email, _faker.Internet.Email())
            .Create();

        var updatedUserDto = _fixture.Create<UserDto>();

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        repoMock.Setup(r => r.GetByUsernameAsync(updateDto.Username!)).ReturnsAsync((User)null!);
        repoMock.Setup(r => r.GetByEmailAsync(updateDto.Email!)).ReturnsAsync((User)null!);
        repoMock.Setup(r => r.UpdateAsync(existingUser)).ReturnsAsync(true);

        var mapperMock = _fixture.Freeze<Mock<IMapper>>();
        mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(updatedUserDto);

        var service = _fixture.Create<UserService>();
        var result = await service.UpdateUserAsync(userId, updateDto);

        result.ShouldNotBeNull();
        result.ShouldBe(updatedUserDto);
        repoMock.Verify(r => r.UpdateAsync(existingUser), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_UserNotFound_ReturnsNull()
    {
        var userId = _faker.Random.Int(1, 1000);
        var updateDto = _fixture.Build<UserUpdateDto>()
            .With(u => u.Username, _faker.Internet.UserName())
            .With(u => u.Email, _faker.Internet.Email())
            .Create();

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null!);

        var service = _fixture.Create<UserService>();
        var result = await service.UpdateUserAsync(userId, updateDto);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateUserAsync_UsernameTaken_ThrowsException()
    {
        var userId = _faker.Random.Int(1, 1000);
        var existingUser = _fixture.Create<User>();
        var takenUser = _fixture.Create<User>();
        var username = _faker.Internet.UserName();

        var updateDto = _fixture.Build<UserUpdateDto>()
            .With(u => u.Username, username)
            .With(u => u.Email, _faker.Internet.Email())
            .Create();

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        repoMock.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync(takenUser);

        var service = _fixture.Create<UserService>();

        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await service.UpdateUserAsync(userId, updateDto));
    }

    [Fact]
    public async Task UpdateUserAsync_EmailTaken_ThrowsException()
    {
        var userId = _faker.Random.Int(1, 1000);
        var existingUser = _fixture.Create<User>();
        var takenUser = _fixture.Create<User>();
        var email = _faker.Internet.Email();

        var updateDto = _fixture.Build<UserUpdateDto>()
            .With(u => u.Username, _faker.Internet.UserName())
            .With(u => u.Email, email)
            .Create();

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        repoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(takenUser);

        var service = _fixture.Create<UserService>();

        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await service.UpdateUserAsync(userId, updateDto));
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsResult()
    {
        var userId = _faker.Random.Int(1, 1000);

        var repoMock = _fixture.Freeze<Mock<IUserRepository>>();
        repoMock.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(true);

        var service = _fixture.Create<UserService>();
        var result = await service.DeleteUserAsync(userId);

        result.ShouldBeTrue();
    }
}
