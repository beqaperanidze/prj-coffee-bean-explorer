using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class ReviewRepository(DbConnectionFactory dbContext) : IReviewRepository
{
    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryAsync<Review, User, Bean, Review>(
            """
            SELECT 
                r."Id", r."UserId", r."BeanId", r."Rating", r."Comment", r."CreatedAt", r."UpdatedAt",
                u."Id", u."Username", u."Email", u."FirstName", u."LastName", u."Role", u."CreatedAt", u."UpdatedAt",
                b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt"
            FROM "Social"."Reviews" r
            JOIN "Auth"."Users" u ON r."UserId" = u."Id"
            JOIN "Product"."Beans" b ON r."BeanId" = b."Id"
            """,
            (review, user, bean) =>
            {
                review.User = user;
                review.Bean = bean;
                return review;
            },
            splitOn: "Id,Id"
        );
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        var reviews = await connection.QueryAsync<Review, User, Bean, Review>(
            """
            SELECT 
                r."Id", r."UserId", r."BeanId", r."Rating", r."Comment", r."CreatedAt", r."UpdatedAt",
                u."Id", u."Username", u."Email", u."FirstName", u."LastName", u."Role", u."CreatedAt", u."UpdatedAt",
                b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt"
            FROM "Social"."Reviews" r
            JOIN "Auth"."Users" u ON r."UserId" = u."Id"
            JOIN "Product"."Beans" b ON r."BeanId" = b."Id"
            WHERE r."Id" = @Id
            """,
            (review, user, bean) =>
            {
                review.User = user;
                review.Bean = bean;
                return review;
            },
            new { Id = id },
            splitOn: "Id,Id"
        );

        return reviews.FirstOrDefault();
    }

    public async Task<IEnumerable<Review>> GetByBeanIdAsync(int beanId)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryAsync<Review, User, Review>(
            """
            SELECT 
                r."Id", r."UserId", r."BeanId", r."Rating", r."Comment", r."CreatedAt", r."UpdatedAt",
                u."Id", u."Username", u."Email", u."FirstName", u."LastName", u."Role", u."CreatedAt", u."UpdatedAt"
            FROM "Social"."Reviews" r
            JOIN "Auth"."Users" u ON r."UserId" = u."Id"
            WHERE r."BeanId" = @BeanId
            """,
            (review, user) =>
            {
                review.User = user;
                return review;
            },
            new { BeanId = beanId },
            splitOn: "Id"
        );
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
    {
        using var connection = dbContext.GetConnection();
        return await connection.QueryAsync<Review, Bean, Review>(
            """
            SELECT 
                r."Id", r."UserId", r."BeanId", r."Rating", r."Comment", r."CreatedAt", r."UpdatedAt",
                b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt"
            FROM "Social"."Reviews" r
            JOIN "Product"."Beans" b ON r."BeanId" = b."Id"
            WHERE r."UserId" = @UserId
            """,
            (review, bean) =>
            {
                review.Bean = bean;
                return review;
            },
            new { UserId = userId },
            splitOn: "Id"
        );
    }

    public async Task<bool> HasUserReviewedBeanAsync(int userId, int beanId)
    {
        using var connection = dbContext.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(
            """
            SELECT COUNT(*) > 0
            FROM "Social"."Reviews"
            WHERE "UserId" = @UserId AND "BeanId" = @BeanId
            """,
            new { UserId = userId, BeanId = beanId });
    }

    public async Task<Review> AddAsync(Review review)
    {
        using var connection = dbContext.GetConnection();
        var insertedReview = await connection.QuerySingleAsync<Review>(
            """
            INSERT INTO "Social"."Reviews" ("UserId", "BeanId", "Rating", "Comment")
            VALUES (@UserId, @BeanId, @Rating, @Comment)
            RETURNING "Id", "UserId", "BeanId", "Rating", "Comment", "CreatedAt", "UpdatedAt"
            """,
            review);

        return insertedReview;
    }

    public async Task<bool> UpdateAsync(Review review)
    {
        using var connection = dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            UPDATE "Social"."Reviews"
            SET "Rating" = @Rating,
                "Comment" = @Comment,
                "UpdatedAt" = now()
            WHERE "Id" = @Id
            """,
            review);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            DELETE FROM "Social"."Reviews"
            WHERE "Id" = @Id
            """,
            new { Id = id });

        return rowsAffected > 0;
    }
}