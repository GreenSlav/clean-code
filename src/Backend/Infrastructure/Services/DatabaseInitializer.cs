using System.Data;
using Npgsql;

namespace Infrastructure.Services;

public class DatabaseInitializer
{
    private readonly string _defaultConnectionString;
    private readonly string _targetDbName;

    public DatabaseInitializer(EnvService envService)
    {
        var host = envService.GetVariable("POSTGRES_HOST");
        var port = envService.GetVariable("POSTGRES_PORT");
        var user = envService.GetVariable("POSTGRES_USER");
        var password = envService.GetVariable("POSTGRES_PASSWORD");
        var defaultDb = envService.GetVariable("POSTGRES_DEFAULT_DB");
        _targetDbName = envService.GetVariable("POSTGRES_TARGET_DB");

        _defaultConnectionString =
            $"Host={host};Port={port};Database={defaultDb};Username={user};Password={password}";
    }

    public void EnsureDatabaseExists()
    {
        using var connection = new NpgsqlConnection(_defaultConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{_targetDbName}';";
        var dbExists = command.ExecuteScalar() != null;

        if (!dbExists)
        {
            command.CommandText = $"CREATE DATABASE \"{_targetDbName}\";";
            command.ExecuteNonQuery();
        }
    }
}