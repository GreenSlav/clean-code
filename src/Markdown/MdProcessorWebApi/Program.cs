using DotNetEnv;
using MdProcessorWebApi.DbContexts;
using MdProcessorWebApi.Models;
using Microsoft.EntityFrameworkCore;

// Переменные из .env выгружаются в окружение приложения
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Разрешение CORS с указанием доверенного источника
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Указываем доверенный источник
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Разрешаем передачу учетных данных
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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