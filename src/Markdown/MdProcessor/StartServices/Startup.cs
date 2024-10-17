using MdProcessor.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MdProcessor.StartServices;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL")));
    }

    public void Configure(IApplicationBuilder app, ApplicationDbContext dbContext)
    {
        // Проверяет и создает базу данных, если она не существует
        dbContext.Database.EnsureCreated();
    }
}