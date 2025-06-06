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
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", 
                   "UpdatedAt", "PasswordHash", "Salt", "LastLogin", "IsActive"
            FROM "Auth"."Users"
            """);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", 
                   "UpdatedAt", "PasswordHash", "Salt", "LastLogin", "IsActive"
            FROM "Auth"."Users"
            WHERE "Id" = @Id
            """,
            new { Id = id });

        if (user != null)
            user.RefreshTokens = (await connection.QueryAsync<RefreshToken>(
                """
                SELECT "Id", "Token", "Expires", "Created", "Revoked", "ReplacedByToken", "ReasonRevoked", "UserId"
                FROM "Auth"."RefreshTokens"
                WHERE "UserId" = @UserId
                """,
                new { UserId = id })).ToList();

        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = dbContext.GetConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", 
                   "UpdatedAt", "PasswordHash", "Salt", "LastLogin", "IsActive"
            FROM "Auth"."Users"
            WHERE "Username" = @Username
            """,
            new { Username = username });

        if (user != null)
            user.RefreshTokens = (await connection.QueryAsync<RefreshToken>(
                """
                SELECT "Id", "Token", "Expires", "Created", "Revoked", "ReplacedByToken", "ReasonRevoked", "UserId"
                FROM "Auth"."RefreshTokens"
                WHERE "UserId" = @UserId
                """,
                new { UserId = user.Id })).ToList();

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = dbContext.GetConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", 
                   "UpdatedAt", "PasswordHash", "Salt", "LastLogin", "IsActive"
            FROM "Auth"."Users"
            WHERE LOWER("Email") = LOWER(@Email)
            """,
            new { Email = email });

        if (user != null)
            user.RefreshTokens = (await connection.QueryAsync<RefreshToken>(
                """
                SELECT "Id", "Token", "Expires", "Created", "Revoked", "ReplacedByToken", "ReasonRevoked", "UserId"
                FROM "Auth"."RefreshTokens"
                WHERE "UserId" = @UserId
                """,
                new { UserId = user.Id })).ToList();

        return user;
    }

    public async Task<(bool UsernameExists, bool EmailExists)> CheckUserExistsAsync(string username, string email)
    {
        using var connection = dbContext.GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<(int UsernameCount, int EmailCount)>(
            """
            SELECT 
                (SELECT COUNT(*) FROM "Auth"."Users" WHERE "Username" = @Username) as UsernameCount,
                (SELECT COUNT(*) FROM "Auth"."Users" WHERE LOWER("Email") = LOWER(@Email)) as EmailCount
            """,
            new { Username = username, Email = email });

        return (result.UsernameCount > 0, result.EmailCount > 0);
    }

    public async Task<User> AddAsync(User user)
    {
        using var connection = dbContext.GetConnection();
        var insertedUserId = await connection.QuerySingleAsync<int>(
            """
            INSERT INTO "Auth"."Users" ("Username", "Email", "FirstName", "LastName", "PasswordHash",
                                        "Salt", "IsActive", "CreatedAt", "UpdatedAt", "Role")
            VALUES (@Username, @Email, @FirstName, @LastName, @PasswordHash,
                    @Salt, @IsActive, now(), now(), @Role::"UserRole")
            RETURNING "Id"
            """,
            new
            {
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PasswordHash,
                user.Salt,
                user.IsActive,
                Role = user.Role.ToString() // Pass enum as string
            });

        user.Id = insertedUserId;

        if (user.RefreshTokens.Count <= 0) return await GetByIdAsync(user.Id) ?? user;
        foreach (var token in user.RefreshTokens)
        {
            token.UserId = user.Id;
            await connection.ExecuteAsync(
                """
                INSERT INTO "Auth"."RefreshTokens" ("Token", "Expires", "Created", "UserId")
                VALUES (@Token, @Expires, @Created, @UserId)
                """,
                token);
        }

        return await GetByIdAsync(user.Id) ?? user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        using var connection = dbContext.GetConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            var rowsAffected = await connection.ExecuteAsync(
                """
                UPDATE "Auth"."Users"
                SET "Username" = @Username,
                    "Email" = @Email,
                    "FirstName" = @FirstName,
                    "LastName" = @LastName,
                    "Salt" = @Salt,
                    "LastLogin" = @LastLogin,
                    "IsActive" = @IsActive,
                    "UpdatedAt" = now()
                WHERE "Id" = @Id
                """,
                user,
                transaction);

            if (user.RefreshTokens.Count != 0)
                foreach (var token in user.RefreshTokens)
                    if (token.Id == 0)
                        await connection.ExecuteAsync(
                            """
                            INSERT INTO "Auth"."RefreshTokens" ("Token", "Expires", "Created", "Revoked", "ReplacedByToken", "ReasonRevoked", "UserId")
                            VALUES (@Token, @Expires, @Created, @Revoked, @ReplacedByToken, @ReasonRevoked, @UserId)
                            """,
                            token,
                            transaction);
                    else
                        await connection.ExecuteAsync(
                            """
                            UPDATE "Auth"."RefreshTokens"
                            SET "Revoked" = @Revoked,
                                "ReplacedByToken" = @ReplacedByToken,
                                "ReasonRevoked" = @ReasonRevoked
                            WHERE "Id" = @Id AND "UserId" = @UserId
                            """,
                            token,
                            transaction);

            transaction.Commit();
            return rowsAffected > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(
                """
                DELETE FROM "Auth"."RefreshTokens" WHERE "UserId" = @Id;
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