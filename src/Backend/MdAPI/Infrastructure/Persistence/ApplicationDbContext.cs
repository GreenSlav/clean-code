using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    // Таблицы
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentCollaborator> DocumentCollaborators { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // // Настройка связи Document ↔ User (Owner)
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Owner) // Один владелец
            .WithMany() // Владелец может владеть многими документами
            .HasForeignKey(d => d.OwnerId) // Внешний ключ
            .OnDelete(DeleteBehavior.Cascade); // Удаление документа при удалении владельца

        // Настройка связи DocumentCollaborator ↔ Document
        modelBuilder.Entity<DocumentCollaborator>()
            .HasOne(dc => dc.Document) // Один документ
            .WithMany(d => d.Collaborators) // У документа много коллабораторов
            .HasForeignKey(dc => dc.DocumentId) // Внешний ключ
            .OnDelete(DeleteBehavior.Cascade); // Удаление коллабораций при удалении документа
        
        // Настройка связи DocumentCollaborator ↔ User
        modelBuilder.Entity<DocumentCollaborator>()
            .HasOne(dc => dc.User) // Один пользователь
            .WithMany() // Пользователь может быть коллаборатором для многих документов
            .HasForeignKey(dc => dc.UserId) // Внешний ключ
            .OnDelete(DeleteBehavior.Cascade); // Удаление коллабораций при удалении пользователя
    }
}