using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _dbContext;

    public UserRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryAsync<User>(
            """
            SELECT * FROM "Auth"."Users"
            """);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT * FROM "Auth"."Users" 
            WHERE "Id" = @Id
            """,
            new { Id = id });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT * FROM "Auth"."Users" 
            WHERE "Username" = @Username
            """,
            new { Username = username });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT * FROM "Auth"."Users" 
            WHERE LOWER("Email") = LOWER(@Email)
            """,
            new { Email = email });
    }

    public async Task<User> AddAsync(User user)
    {
        using var connection = _dbContext.GetConnection();
        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO "Auth"."Users" ("Username", "Email", "FirstName", "LastName", "PasswordHash")
            VALUES (@Username, @Email, @FirstName, @LastName, @PasswordHash)
            RETURNING "Id"
            """,
            user);

        user.Id = id;
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        using var connection = _dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            UPDATE "Auth"."Users"
            SET "Username" = @Username,
                "Email" = @Email,
                "FirstName" = @FirstName,
                "LastName" = @LastName,
                "UpdatedAt" = now()
            WHERE "Id" = @Id
            """,
            user);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _dbContext.GetConnection();

        await connection.ExecuteAsync(
            """DELETE FROM "Social"."UserLists" WHERE "UserId" = @Id""",
            new { Id = id });

        await connection.ExecuteAsync(
            """DELETE FROM "Social"."Reviews" WHERE "UserId" = @Id""",
            new { Id = id });

        var rowsAffected = await connection.ExecuteAsync(
            """DELETE FROM "Auth"."Users" WHERE "Id" = @Id""",
            new { Id = id });

        return rowsAffected > 0;
    }
}