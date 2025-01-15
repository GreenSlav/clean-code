namespace Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; } // Markdown содержимое
    public Guid OwnerId { get; private set; } // Владелец документа
    public bool IsPrivate { get; private set; } // Приватность документа
    public DateTime LastEdited { get; private set; } // Последнее редактирование

    // Навигационные свойства
    public User Owner { get; private set; } // Владелец документа
    public ICollection<DocumentCollaborator> Collaborators { get; private set; } = new List<DocumentCollaborator>();

    // Публичный конструктор
    public Document(string title, string content, Guid ownerId, bool isPrivate = true)
    {
        Id = Guid.NewGuid();
        Title = title;
        Content = content;
        OwnerId = ownerId;
        IsPrivate = isPrivate;
        LastEdited = DateTime.UtcNow;
    }

    // Приватный конструктор для EF Core
    private Document() { }

    // Метод для обновления содержимого
    public void UpdateContent(string newContent)
    {
        Content = newContent;
        LastEdited = DateTime.UtcNow;
    }

    // Метод для изменения приватности
    public void SetPrivacy(bool isPrivate)
    {
        IsPrivate = isPrivate;
    }
}