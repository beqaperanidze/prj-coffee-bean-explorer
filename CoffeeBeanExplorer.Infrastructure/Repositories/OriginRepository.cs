using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class OriginRepository : IOriginRepository
{
    private readonly DatabaseContext _dbContext;

    public OriginRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Origin>> GetAllAsync()
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryAsync<Origin>(
            """
            SELECT * FROM "Product"."Origins"
            """);
    }

    public async Task<Origin?> GetByIdAsync(int id)
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QuerySingleOrDefaultAsync<Origin>(
            """SELECT * FROM "Product"."Origins" WHERE "Id" = @Id""",
            new { Id = id });
    }

    public async Task<Origin> AddAsync(Origin origin)
    {
        using var connection = _dbContext.GetConnection();
        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO "Product"."Origins" ("Country", "Region")
                                VALUES (@Country, @Region)
                                RETURNING "Id"
            """,
            origin);

        origin.Id = id;
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> UpdateAsync(Origin origin)
    {
        using var connection = _dbContext.GetConnection();
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
        int beansUsingOrigin;
        using (var connection = _dbContext.GetConnection())
        {
            beansUsingOrigin = await connection.ExecuteScalarAsync<int>(
                """SELECT COUNT(*) FROM "Product"."Beans" WHERE "OriginId" = @Id""",
                new { Id = id });
        }

        if (beansUsingOrigin > 0)
        {
            return false;
        }

        using (var connection = _dbContext.GetConnection())
        {
            var rowsAffected = await connection.ExecuteAsync(
                """DELETE FROM "Product"."Origins" WHERE "Id" = @Id""",
                new { Id = id });

            return rowsAffected > 0;
        }
    }
}