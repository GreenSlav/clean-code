namespace Domain.Entities;

public class DocumentCollaborator
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; } // Роли: Viewer, Editor, Owner

    // Навигационные свойства
    public Document Document { get; private set; }
    public User User { get; private set; }

    // Публичный конструктор
    public DocumentCollaborator(Guid documentId, Guid userId, string role)
    {
        Id = Guid.NewGuid();
        DocumentId = documentId;
        UserId = userId;
        Role = role;
    }

    // Приватный конструктор для EF Core
    private DocumentCollaborator() { }
    
    public void UpdateRole(string newRole)
    {
        if (newRole == "viewer" || newRole == "editor")
        {
            Role = newRole;
        }
    }
}