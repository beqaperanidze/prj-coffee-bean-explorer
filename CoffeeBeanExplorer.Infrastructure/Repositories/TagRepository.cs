using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class TagRepository(DbConnectionFactory dbContext) : ITagRepository
{
    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryAsync<Tag>(
            """
            SELECT "Id", "Name", "CreatedAt", "UpdatedAt"
            FROM "Product"."Tags"
            """);
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QuerySingleOrDefaultAsync<Tag>(
            """
            SELECT "Id", "Name", "CreatedAt", "UpdatedAt"
            FROM "Product"."Tags" 
            WHERE "Id" = @Id
            """,
            new { Id = id });
    }

    public async Task<IEnumerable<Tag>> GetByBeanIdAsync(int beanId)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryAsync<Tag>(
            """
            SELECT t."Id", t."Name", t."CreatedAt", t."UpdatedAt"
            FROM "Product"."Tags" t
            JOIN "Product"."BeansTags" bt ON t."Id" = bt."TagId"
            WHERE bt."BeanId" = @BeanId
            """,
            new { BeanId = beanId });
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        using var connection = dbContext.GetConnection();
        var insertedTag = await connection.QuerySingleAsync<Tag>(
            """
            INSERT INTO "Product"."Tags" ("Name")
            VALUES (@Name)
            RETURNING "Id", "Name", "CreatedAt", "UpdatedAt"
            """,
            tag);

        return insertedTag;
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        using var connection = dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            UPDATE "Product"."Tags"
            SET "Name" = @Name,
                "UpdatedAt" = now()
            WHERE "Id" = @Id
            """,
            tag);

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
                DELETE FROM "Product"."BeansTags" WHERE "TagId" = @Id;
                DELETE FROM "Product"."Tags" WHERE "Id" = @Id;
                """,
                new { Id = id },
                transaction);

            transaction.Commit();
            var exists = await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(*) > 0 FROM \"Product\".\"Tags\" WHERE \"Id\" = @Id",
                new { Id = id });

            return !exists;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> AddTagToBeanAsync(int tagId, int beanId)
    {
        using var connection = dbContext.GetConnection();

        var exists = await connection.ExecuteScalarAsync<bool>(
            """
            SELECT COUNT(*) > 0
            FROM "Product"."BeansTags" 
            WHERE "TagId" = @TagId AND "BeanId" = @BeanId
            """,
            new { TagId = tagId, BeanId = beanId });

        if (exists)
        {
            return false;
        }

        var rowsAffected = await connection.ExecuteAsync(
            """
            INSERT INTO "Product"."BeansTags" ("TagId", "BeanId")
            VALUES (@TagId, @BeanId)
            """,
            new { TagId = tagId, BeanId = beanId });

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveTagFromBeanAsync(int tagId, int beanId)
    {
        using var connection = dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            DELETE FROM "Product"."BeansTags" 
            WHERE "TagId" = @TagId AND "BeanId" = @BeanId
            """,
            new { TagId = tagId, BeanId = beanId });

        return rowsAffected > 0;
    }
}