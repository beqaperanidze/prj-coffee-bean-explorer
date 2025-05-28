using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class BeanRepository : IBeanRepository
{
    private readonly DatabaseContext _dbContext;

    public BeanRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Bean>> GetAllAsync()
    {
        using var connection = _dbContext.GetConnection();

        var beanDict = new Dictionary<int, Bean>();

        await connection.QueryAsync<Bean, Origin, Tag, Bean>(
            """
            SELECT b.*, o.*, t.*
            FROM "Product"."Beans" b
            LEFT JOIN "Product"."Origins" o ON b."OriginId" = o."Id"
            LEFT JOIN "Product"."BeansTags" bt ON b."Id" = bt."BeanId"
            LEFT JOIN "Product"."Tags" t ON bt."TagId" = t."Id"
            ORDER BY b."Id"
            """,
            (bean, origin, tag) =>
            {
                if (!beanDict.TryGetValue(bean.Id, out var existingBean))
                {
                    bean.Origin = origin;
                    bean.BeanTags = new List<BeanTag>();
                    beanDict.Add(bean.Id, bean);
                    existingBean = bean;
                }

                if (tag != null && tag.Id != 0)
                {
                    existingBean.BeanTags.Add(new BeanTag
                    {
                        BeanId = bean.Id,
                        TagId = tag.Id,
                        Tag = tag
                    });
                }

                return existingBean;
            },
            splitOn: "Id,Id"
        );

        return beanDict.Values;
    }

    public async Task<Bean?> GetByIdAsync(int id)
    {
        using var connection = _dbContext.GetConnection();

        Bean? result = null;

        await connection.QueryAsync<Bean, Origin, Tag, Bean>(
            """
            SELECT b.*, o.*, t.*
            FROM "Product"."Beans" b
            LEFT JOIN "Product"."Origins" o ON b."OriginId" = o."Id"
            LEFT JOIN "Product"."BeansTags" bt ON b."Id" = bt."BeanId"
            LEFT JOIN "Product"."Tags" t ON bt."TagId" = t."Id"
            WHERE b."Id" = @Id
            ORDER BY b."Id"
            """,
            (bean, origin, tag) =>
            {
                if (result == null)
                {
                    result = bean;
                    result.Origin = origin;
                    result.BeanTags = new List<BeanTag>();
                }

                if (tag != null && tag.Id != 0)
                {
                    result.BeanTags.Add(new BeanTag
                    {
                        BeanId = bean.Id,
                        TagId = tag.Id,
                        Tag = tag
                    });
                }

                return result;
            },
            new { Id = id },
            splitOn: "Id,Id"
        );
        Console.WriteLine("          " + (result?.BeanTags.Count ?? 0));
        return result;
    }

    public async Task<Bean> AddAsync(Bean bean)
    {
        using var connection = _dbContext.GetConnection();
        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO "Product"."Beans" ("Name", "OriginId", "RoastLevel", "Description", "Price")
            VALUES (@Name, @OriginId, @RoastLevel::text::"RoastLevel", @Description, @Price)
            RETURNING "Id"
            """,
            new
            {
                bean.Name,
                bean.OriginId,
                RoastLevel = bean.RoastLevel.ToString(),
                bean.Description,
                bean.Price
            });

        bean.Id = id;
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> UpdateAsync(Bean bean)
    {
        using var connection = _dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            UPDATE "Product"."Beans"
            SET "Name" = @Name,
                "OriginId" = @OriginId,
                "RoastLevel" = @RoastLevel::text::"RoastLevel",
                "Description" = @Description,
                "Price" = @Price,
                "UpdatedAt" = now()
            WHERE "Id" = @Id
            """,
            new
            {
                bean.Id,
                bean.Name,
                bean.OriginId,
                RoastLevel = bean.RoastLevel.ToString(),
                bean.Description,
                bean.Price
            });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _dbContext.GetConnection();

        await connection.ExecuteAsync(
            """DELETE FROM "Product"."BeansTags" WHERE "BeanId" = @Id""",
            new { Id = id });

        await connection.ExecuteAsync(
            """DELETE FROM "Social"."Reviews" WHERE "BeanId" = @Id""",
            new { Id = id });

        var rowsAffected = await connection.ExecuteAsync(
            """DELETE FROM "Product"."Beans" WHERE "Id" = @Id""",
            new { Id = id });

        return rowsAffected > 0;
    }
}