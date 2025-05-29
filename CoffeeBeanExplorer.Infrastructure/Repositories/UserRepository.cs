using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class UserRepository(DbConnectionFactory dbContext) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", "UpdatedAt", "PasswordHash" 
            FROM "Auth"."Users"
            """);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", "UpdatedAt", "PasswordHash"
            FROM "Auth"."Users"
            WHERE "Id" = @Id
            """,
            new { Id = id });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", "UpdatedAt", "PasswordHash"
            FROM "Auth"."Users"
            WHERE "Username" = @Username
            """,
            new { Username = username });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", "UpdatedAt", "PasswordHash"
            FROM "Auth"."Users"
            WHERE LOWER("Email") = LOWER(@Email)
            """,
            new { Email = email });
    }

    public async Task<User> AddAsync(User user)
    {
        using var connection = dbContext.GetConnection();
        var insertedUser = await connection.QuerySingleAsync<User>(
            """
            INSERT INTO "Auth"."Users" ("Username", "Email", "FirstName", "LastName", "PasswordHash")
            VALUES (@Username, @Email, @FirstName, @LastName, @PasswordHash)
            RETURNING "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", "UpdatedAt", "PasswordHash"
            """,
            user);

        return insertedUser;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        using var connection = dbContext.GetConnection();
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
        using var connection = dbContext.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(
                """
                DELETE FROM "Social"."UserLists" WHERE "UserId" = @Id;
                DELETE FROM "Social"."Reviews" WHERE "UserId" = @Id;
                DELETE FROM "Auth"."Users" WHERE "Id" = @Id;
                """,
                new { Id = id },
                transaction);

            transaction.Commit();
            var exists = await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(*) > 0 FROM \"Auth\".\"Users\" WHERE \"Id\" = @Id",
                new { Id = id });

            return !exists;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}