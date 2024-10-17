using DotNetEnv;
using MdProcessor.DbContexts;
using MdProcessor.Models;
using Microsoft.EntityFrameworkCore;

// Переменные из .env выгружаются в окружение приложения
Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

// Добавляем контекст базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Добавляем контроллеры
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
   
// Настройка эндпоинтов
app.UseEndpoints(endpoints =>
{
   endpoints.MapControllers();
});

app.Run();