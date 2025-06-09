using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using Dapper;

namespace CoffeeBeanExplorer.Infrastructure.Repositories;

public class UserListRepository(DbConnectionFactory dbContext) : IUserListRepository
{
    public async Task<IEnumerable<UserList>> GetAllAsync()
    {
        using var connection = dbContext.GetConnection();
        var userListDictionary = new Dictionary<int, UserList>();

        await connection.QueryAsync<UserList, User, UserList>(
            """
            SELECT 
                ul."Id", ul."UserId", ul."Name", ul."CreatedAt", ul."UpdatedAt",
                u."Id", u."Username", u."Email", u."FirstName", u."LastName", u."Role", u."CreatedAt", u."UpdatedAt"
            FROM "Social"."UserLists" ul
            JOIN "Auth"."Users" u ON ul."UserId" = u."Id"
            """,
            (userList, user) =>
            {
                userList.User = user;
                userListDictionary[userList.Id] = userList;
                return userList;
            },
            splitOn: "Id"
        );

        foreach (var userList in userListDictionary.Values)
        {
            var items = await connection.QueryAsync<ListItem, Bean, Origin, ListItem>(
                """
                SELECT 
                    li."ListId", li."BeanId", li."CreatedAt",
                    b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt",
                    o."Id", o."Country", o."Region", o."CreatedAt", o."UpdatedAt"
                FROM "Social"."ListItems" li
                JOIN "Product"."Beans" b ON li."BeanId" = b."Id"
                LEFT JOIN "Product"."Origins" o ON b."OriginId" = o."Id"
                WHERE li."ListId" = @ListId
                """,
                (listItem, bean, origin) =>
                {
                    bean.Origin = origin;
                    listItem.Bean = bean;
                    return listItem;
                },
                new { ListId = userList.Id },
                splitOn: "Id,Id"
            );

            foreach (var item in items) userList.Items.Add(item);
        }

        return userListDictionary.Values;
    }

    public async Task<UserList?> GetByIdAsync(int id)
    {
        using var connection = dbContext.GetConnection();
        var userList = await connection.QuerySingleOrDefaultAsync<UserList>(
            """
            SELECT "Id", "UserId", "Name", "CreatedAt", "UpdatedAt"
            FROM "Social"."UserLists"
            WHERE "Id" = @Id
            """,
            new { Id = id });

        if (userList == null)
            return null;

        userList.User = await connection.QuerySingleOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", "UpdatedAt"
            FROM "Auth"."Users"
            WHERE "Id" = @UserId
            """,
            new { userList.UserId });

        var items = await connection.QueryAsync<ListItem, Bean, Origin, ListItem>(
            """
            SELECT 
                li."ListId", li."BeanId", li."CreatedAt",
                b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt",
                o."Id", o."Country", o."Region", o."CreatedAt", o."UpdatedAt"
            FROM "Social"."ListItems" li
            JOIN "Product"."Beans" b ON li."BeanId" = b."Id"
            LEFT JOIN "Product"."Origins" o ON b."OriginId" = o."Id"
            WHERE li."ListId" = @ListId
            """,
            (listItem, bean, origin) =>
            {
                bean.Origin = origin;
                listItem.Bean = bean;
                return listItem;
            },
            new { ListId = id },
            splitOn: "Id,Id"
        );

        foreach (var item in items) userList.Items.Add(item);

        return userList;
    }

    public async Task<IEnumerable<UserList>> GetByUserIdAsync(int userId)
    {
        using var connection = dbContext.GetConnection();
        var userListDictionary = new Dictionary<int, UserList>();

        var userLists = await connection.QueryAsync<UserList>(
            """
            SELECT "Id", "UserId", "Name", "CreatedAt", "UpdatedAt"
            FROM "Social"."UserLists"
            WHERE "UserId" = @UserId
            """,
            new { UserId = userId });

        foreach (var userList in userLists) userListDictionary[userList.Id] = userList;

        var user = await connection.QuerySingleOrDefaultAsync<User>(
            """
            SELECT "Id", "Username", "Email", "FirstName", "LastName", "Role", "CreatedAt", "UpdatedAt"
            FROM "Auth"."Users"
            WHERE "Id" = @UserId
            """,
            new { UserId = userId });

        foreach (var userList in userListDictionary.Values)
        {
            userList.User = user;

            var items = await connection.QueryAsync<ListItem, Bean, Origin, ListItem>(
                """
                SELECT 
                    li."ListId", li."BeanId", li."CreatedAt",
                    b."Id", b."Name", b."OriginId", b."RoastLevel", b."Description", b."Price", b."CreatedAt", b."UpdatedAt",
                    o."Id", o."Country", o."Region", o."CreatedAt", o."UpdatedAt"
                FROM "Social"."ListItems" li
                JOIN "Product"."Beans" b ON li."BeanId" = b."Id"
                LEFT JOIN "Product"."Origins" o ON b."OriginId" = o."Id"
                WHERE li."ListId" = @ListId
                """,
                (listItem, bean, origin) =>
                {
                    bean.Origin = origin;
                    listItem.Bean = bean;
                    return listItem;
                },
                new { ListId = userList.Id },
                splitOn: "Id,Id"
            );

            foreach (var item in items) userList.Items.Add(item);
        }

        return userListDictionary.Values;
    }

    public async Task<UserList> AddAsync(UserList userList)
    {
        using var connection = dbContext.GetConnection();
        var insertedUserList = await connection.QuerySingleAsync<UserList>(
            """
            INSERT INTO "Social"."UserLists" ("UserId", "Name")
            VALUES (@UserId, @Name)
            RETURNING "Id", "UserId", "Name", "CreatedAt", "UpdatedAt"
            """,
            userList);

        return insertedUserList;
    }

    public async Task<bool> UpdateAsync(UserList userList)
    {
        using var connection = dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            UPDATE "Social"."UserLists"
            SET "Name" = @Name,
                "UpdatedAt" = now()
            WHERE "Id" = @Id
            """,
            userList);

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
                DELETE FROM "Social"."ListItems" WHERE "ListId" = @Id;
                DELETE FROM "Social"."UserLists" WHERE "Id" = @Id;
                """,
                new { Id = id },
                transaction);

            transaction.Commit();
            var exists = await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(*) > 0 FROM \"Social\".\"UserLists\" WHERE \"Id\" = @Id",
                new { Id = id });

            return !exists;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> AddBeanToListAsync(int listId, int beanId)
    {
        using var connection = dbContext.GetConnection();

        var exists = await connection.ExecuteScalarAsync<bool>(
            """
            SELECT COUNT(*) > 0
            FROM "Social"."ListItems"
            WHERE "ListId" = @ListId AND "BeanId" = @BeanId
            """,
            new { ListId = listId, BeanId = beanId });

        if (exists) return false;

        var rowsAffected = await connection.ExecuteAsync(
            """
            INSERT INTO "Social"."ListItems" ("ListId", "BeanId")
            VALUES (@ListId, @BeanId)
            """,
            new { ListId = listId, BeanId = beanId });

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveBeanFromListAsync(int listId, int beanId)
    {
        using var connection = dbContext.GetConnection();
        var rowsAffected = await connection.ExecuteAsync(
            """
            DELETE FROM "Social"."ListItems"
            WHERE "ListId" = @ListId AND "BeanId" = @BeanId
            """,
            new { ListId = listId, BeanId = beanId });

        return rowsAffected > 0;
    }

    public async Task<bool> IsBeanInListAsync(int listId, int beanId)
    {
        using var connection = dbContext.GetConnection();
        var sql = @"SELECT COUNT(*) > 0 
                FROM ""Social"".""ListItems"" 
                WHERE ""ListId"" = @ListId AND ""BeanId"" = @BeanId";

        return await connection.ExecuteScalarAsync<bool>(sql, new { ListId = listId, BeanId = beanId });
    }
}