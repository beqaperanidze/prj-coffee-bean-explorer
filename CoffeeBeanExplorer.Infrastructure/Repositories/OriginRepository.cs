using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class OriginRepository(DbConnectionFactory dbContext) : IOriginRepository
{
    public async Task<IEnumerable<Origin>> GetAllAsync()
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryAsync<Origin>(
            """
            SELECT "Id", "Country", "Region", "CreatedAt", "UpdatedAt"
            FROM "Product"."Origins"
            """);
    }

    public async Task<Origin?> GetByIdAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QuerySingleOrDefaultAsync<Origin>(
            """
            SELECT "Id", "Country", "Region", "CreatedAt", "UpdatedAt" 
            FROM "Product"."Origins" 
            WHERE "Id" = @Id
            """,
            new { Id = id });
    }

    public async Task<Origin> AddAsync(Origin origin)
    {
        using var connection = dbContext.GetConnection();
        var insertedOrigin = await connection.QuerySingleAsync<Origin>(
            """
            INSERT INTO "Product"."Origins" ("Country", "Region")
            VALUES (@Country, @Region)
            RETURNING "Id", "Country", "Region", "CreatedAt", "UpdatedAt"
            """,
            origin);

        return insertedOrigin;
    }

    public async Task<bool> UpdateAsync(Origin origin)
    {
        using var connection = dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            UPDATE "Product"."Origins"
                        SET "Country" = @Country,
                            "Region" = @Region,
                            "UpdatedAt" = now()
                        WHERE "Id" = @Id
            """,
            origin);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = dbContext.GetConnection();

        var beansUsingOrigin = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Product\".\"Beans\" WHERE \"OriginId\" = @Id",
            new { Id = id });

        if (beansUsingOrigin > 0)
        {
            return false;
        }

        var rowsAffected = await connection.ExecuteAsync(
            "DELETE FROM \"Product\".\"Origins\" WHERE \"Id\" = @Id",
            new { Id = id });

        return rowsAffected > 0;
    }
}