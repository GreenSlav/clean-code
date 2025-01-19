namespace Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; } // Markdown содержимое
    public string S3Path { get; private set; } // Путь к файлу в MinIO
    public Guid OwnerId { get; private set; } // Владелец документа
    public bool IsPrivate { get; private set; } // Приватность документа
    public DateTime LastEdited { get; private set; } // Последнее редактирование

    public User Owner { get; private set; }
    public ICollection<DocumentCollaborator> Collaborators { get; private set; } = new List<DocumentCollaborator>();

    public Document(string title, string content, Guid ownerId, string s3Path, bool isPrivate = true)
    {
        Id = Guid.NewGuid();
        Title = title;
        Content = content;
        S3Path = s3Path;
        OwnerId = ownerId;
        IsPrivate = isPrivate;
        LastEdited = DateTime.UtcNow;
    }

    private Document() { }

    public void UpdateContent(string newContent, string s3Path)
    {
        Content = newContent;
        S3Path = s3Path;
        LastEdited = DateTime.UtcNow;
    }
}