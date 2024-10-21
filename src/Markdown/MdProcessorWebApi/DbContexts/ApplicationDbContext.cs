using MdProcessorWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MdProcessorWebApi.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }  // Таблица User

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Укажи строку подключения к PostgreSQL
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL"));
    }
}