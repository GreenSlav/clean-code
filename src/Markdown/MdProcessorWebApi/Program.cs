using DotNetEnv;
using MdProcessorWebApi.DbContexts;
using MdProcessorWebApi.Models;
using Microsoft.EntityFrameworkCore;

// Переменные из .env выгружаются в окружение приложения
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Разрешение CORS с указанием доверенного источника
var frontendUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS"); //?? "http://localhost:3000"; // Используйте правильный URL
Console.WriteLine("---------- " + frontendUrl);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(/*frontendUrl,*/ "http://frontend:80") // Добавьте необходимый URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

// Добавляем контекст базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Добавляем контроллеры
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

// Включаем CORS
app.UseCors("AllowSpecificOrigin");

// Подача статических файлов
app.UseStaticFiles(); // Это позволит обслуживать файлы из папки wwwroot

app.UseRouting();

app.UseAuthorization();
   
// Настройка эндпоинтов
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();