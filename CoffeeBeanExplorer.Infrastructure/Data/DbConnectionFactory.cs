using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CoffeeBeanExplorer.Infrastructure.Data;

public class DbConnectionFactory(IConfiguration configuration)
{
    public IDbConnection GetConnection()
    {
        var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        connection.Open();
        return connection;
    }
}