using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CoffeeBeanExplorer.Infrastructure.Data;

public class DatabaseContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection GetConnection()
    {
        var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        connection.Open();
        return connection;
    }
}