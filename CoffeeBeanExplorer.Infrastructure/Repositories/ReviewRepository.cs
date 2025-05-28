using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly DatabaseContext _dbContext;

    public ReviewRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        using var connection = _dbContext.GetConnection();
        return await connection.QueryAsync<Review, User, Bean, Review>(
            """
            SELECT r.*, u.*, b.*
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
        using var connection = _dbContext.GetConnection();
        var reviews = await connection.QueryAsync<Review, User, Bean, Review>(
            """
            SELECT r.*, u.*, b.*
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
        using var connection = _dbContext.GetConnection();
        return await connection.QueryAsync<Review, User, Review>(
            """
            SELECT r.*, u.*
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
        using var connection = _dbContext.GetConnection();
        return await connection.QueryAsync<Review, Bean, Review>(
            """
            SELECT r.*, b.*
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
        using var connection = _dbContext.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(
            """
            SELECT COUNT(1) > 0
            FROM "Social"."Reviews"
            WHERE "UserId" = @UserId AND "BeanId" = @BeanId
            """,
            new { UserId = userId, BeanId = beanId });
    }

    public async Task<Review> AddAsync(Review review)
    {
        using var connection = _dbContext.GetConnection();
        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO "Social"."Reviews" ("UserId", "BeanId", "Rating", "Comment")
            VALUES (@UserId, @BeanId, @Rating, @Comment)
            RETURNING "Id"
            """,
            review);

        review.Id = id;
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> UpdateAsync(Review review)
    {
        using var connection = _dbContext.GetConnection();
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
        using var connection = _dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            DELETE FROM "Social"."Reviews" 
            WHERE "Id" = @Id
            """,
            new { Id = id });

        return rowsAffected > 0;
    }
}