using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string envFilePath)
    {
        // Регистрируем EnvVariableLoader
        var envLoader = new EnvService(envFilePath);
        services.AddSingleton(envLoader);

        // Проверяем или создаём базу данных
        var dbInitializer = new DatabaseInitializer(envLoader);
        dbInitializer.EnsureDatabaseExists();

        // Регистрируем DbContext
        var connectionString = BuildConnectionString(envLoader);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Регистрируем репозиторий
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static string BuildConnectionString(EnvService envLoader)
    {
        var host = envLoader.GetVariable("POSTGRES_HOST");
        var port = envLoader.GetVariable("POSTGRES_PORT");
        var user = envLoader.GetVariable("POSTGRES_USER");
        var password = envLoader.GetVariable("POSTGRES_PASSWORD");
        var database = envLoader.GetVariable("POSTGRES_TARGET_DB");

        return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
    }
}