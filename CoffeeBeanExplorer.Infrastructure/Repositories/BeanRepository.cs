using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class BeanRepository(DbConnectionFactory dbContext) : IBeanRepository
{
    public async Task<IEnumerable<Bean>> GetAllAsync()
    {
        using var connection = dbContext.GetConnection();

        var beansById = new Dictionary<int, Bean>();

        await connection.QueryAsync<Bean, Origin, Tag, Bean>(
            """
            SELECT
                b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt",
                o."Id", o."Country", o."Region",
                t."Id", t."Name"
            FROM "Product"."Beans" b
            INNER JOIN "Product"."Origins" o ON b."OriginId" = o."Id"
            LEFT JOIN "Product"."BeansTags" bt ON b."Id" = bt."BeanId"
            LEFT JOIN "Product"."Tags" t ON bt."TagId" = t."Id"
            ORDER BY b."Id"
            """,
            (bean, origin, tag) =>
            {
                if (!beansById.TryGetValue(bean.Id, out var existingBean))
                {
                    bean.Origin = origin;
                    bean.BeanTags = new List<BeanTag>();
                    beansById.Add(bean.Id, bean);
                    existingBean = bean;
                }

                if (tag != null && tag.Id != 0)
                    existingBean.BeanTags.Add(new BeanTag
                    {
                        BeanId = bean.Id,
                        TagId = tag.Id,
                        Tag = tag
                    });

                return existingBean;
            },
            splitOn: "Id,Id"
        );

        return beansById.Values;
    }

    public async Task<Bean?> GetByIdAsync(int id)
    {
        using var connection = dbContext.GetConnection();

        Bean? result = null;

        await connection.QueryAsync<Bean, Origin, Tag, Bean>(
            """
            SELECT
                b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt",
                o."Id", o."Country", o."Region",
                t."Id", t."Name"
            FROM "Product"."Beans" b
            INNER JOIN "Product"."Origins" o ON b."OriginId" = o."Id"
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
                    result.BeanTags.Add(new BeanTag
                    {
                        BeanId = bean.Id,
                        TagId = tag.Id,
                        Tag = tag
                    });

                return result;
            },
            new { Id = id },
            splitOn: "Id,Id"
        );
        return result;
    }

    public async Task<Bean> AddAsync(Bean bean)
    {
        using var connection = dbContext.GetConnection();
        var insertedBean = await connection.QuerySingleAsync<Bean>(
            """
            INSERT INTO "Product"."Beans" ("Name", "OriginId", "RoastLevel", "Description", "Price")
            VALUES (@Name, @OriginId, @RoastLevel::text::"RoastLevel", @Description, @Price)
            RETURNING "Id", "Name", "OriginId", "RoastLevel", "Description", "Price", "CreatedAt", "UpdatedAt"
            """,
            new
            {
                bean.Name,
                bean.OriginId,
                RoastLevel = bean.RoastLevel.ToString(),
                bean.Description,
                bean.Price
            });

        return insertedBean;
    }

    public async Task<bool> UpdateAsync(Bean bean)
    {
        using var connection = dbContext.GetConnection();
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
        using var connection = dbContext.GetConnection();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            """
            DELETE FROM "Product"."BeansTags" WHERE "BeanId" = @Id;
            DELETE FROM "Social"."Reviews" WHERE "BeanId" = @Id;
            DELETE FROM "Product"."Beans" WHERE "Id" = @Id;
            """,
            new { Id = id },
            transaction);

        transaction.Commit();

        var exists = await connection.ExecuteScalarAsync<bool>(
            "SELECT COUNT(*) > 0 FROM \"Product\".\"Beans\" WHERE \"Id\" = @Id",
            new { Id = id });

        return !exists;
    }
}