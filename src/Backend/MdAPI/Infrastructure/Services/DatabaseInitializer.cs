using System.Data;
using Npgsql;

namespace Infrastructure.Services;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(EnvService envService)
    {
        _connectionString = envService.GetVariable("CONNECTION_STRING"); 
    }

    public async Task EnsureDatabaseExistsAsync()
    {
        var defaultConnectionString = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Database = "postgres"
        }.ConnectionString;

        var targetDbName = new NpgsqlConnectionStringBuilder(_connectionString).Database;
        
        using var connection = new NpgsqlConnection(defaultConnectionString);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{targetDbName}';";
        var dbExists = await command.ExecuteScalarAsync() != null;

        if (!dbExists)
        {
            command.CommandText = $"CREATE DATABASE \"{targetDbName}\";";
            await command.ExecuteNonQueryAsync();
        }
    }
}