using Application.Configuration;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем Application-слой
builder.Services.AddApplication();

// Регистрируем инфраструктуру
builder.Services.AddInfrastructure(".env");

builder.Services.AddAuthorization();
builder.Services.AddControllers();


var app = builder.Build();

// Применяем миграции
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Автоматическое применение миграций
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();