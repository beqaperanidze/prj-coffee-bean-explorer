using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly DatabaseContext _dbContext;

    public TagRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryAsync<Tag>(
            """
            SELECT * FROM "Product"."Tags"
            """);
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QuerySingleOrDefaultAsync<Tag>(
            """
            SELECT * FROM "Product"."Tags" 
            WHERE "Id" = @Id
            """, 
            new { Id = id });
    }

    public async Task<IEnumerable<Tag>> GetByBeanIdAsync(int beanId)
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryAsync<Tag>(
            """
            SELECT t.* 
            FROM "Product"."Tags" t
            JOIN "Product"."BeansTags" bt ON t."Id" = bt."TagId"
            WHERE bt."BeanId" = @BeanId
            """,
            new { BeanId = beanId });
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        using var connection = _dbContext.GetConnection();
        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO "Product"."Tags" ("Name")
            VALUES (@Name)
            RETURNING "Id"
            """,
            tag);

        tag.Id = id;
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        using var connection = _dbContext.GetConnection();
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
        using var connection = _dbContext.GetConnection();
        
        await connection.ExecuteAsync(
            """DELETE FROM "Product"."BeansTags" WHERE "TagId" = @Id""",
            new { Id = id });
        
        var rowsAffected = await connection.ExecuteAsync(
            """DELETE FROM "Product"."Tags" WHERE "Id" = @Id""",
            new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddTagToBeanAsync(int tagId, int beanId)
    {
        using var connection = _dbContext.GetConnection();
        
        var exists = await connection.ExecuteScalarAsync<bool>(
            """
            SELECT COUNT(1) > 0
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
        using var connection = _dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            DELETE FROM "Product"."BeansTags" 
            WHERE "TagId" = @TagId AND "BeanId" = @BeanId
            """,
            new { TagId = tagId, BeanId = beanId });
            
        return rowsAffected > 0;
    }
}