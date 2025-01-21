using System.Text;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var envLoader = new EnvService();
builder.Services.AddSingleton(envLoader);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = envLoader.GetVariable("ISSUER", "issuer"),
            ValidAudience = envLoader.GetVariable("AUDIENCE", "audience"),
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envLoader.GetVariable("SECRETKEY", "secretkey")))
        };

        // Настраиваем чтение токена из куки
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken"))
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                }

                return Task.CompletedTask;
            }
        };
    });

// Регистрируем Application-слой
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<AuthService>(sp =>
{
    var envService = sp.GetRequiredService<EnvService>();
    return new AuthService(
        envService.GetVariable("ISSUER", "issuer"),
        envService.GetVariable("AUDIENCE", "audience"),
        envService.GetVariable("SECRETKEY", "secretkey")
    );
});
builder.Services.AddScoped<DocumentService>();



// Регистрируем Infrastructure-слой
// Проверяем или создаём базу данных
var dbInitializer = new DatabaseInitializer(envLoader);
await dbInitializer.EnsureDatabaseExistsAsync(); // Асинхронный вызов

// Регистрируем DbContext
var connectionString = envLoader.GetVariable("CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрируем репозиторий
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IFileStorageRepository>((sp) =>
{
    var envService = sp.GetRequiredService<EnvService>();
    var endpoint = envService.GetVariable("MINIO_ENDPOINT", "localhost:9000");
    var accessKey = envService.GetVariable("MINIO_ACCESS_KEY", "admin");
    var secretKey = envService.GetVariable("MINIO_SECRET_KEY", "admin123");
    
    return new MinioFileStorageRepository(endpoint, accessKey, secretKey);
});

var app = builder.Build();

// Применяем миграции
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync(); // Асинхронная миграция
}

app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync(); // Асинхронный запуск приложения