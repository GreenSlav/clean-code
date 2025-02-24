namespace Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsPrivate { get; private set; }
    public DateTime LastEdited { get; private set; }
    public string S3Path { get; private set; } 

    // Навигационные свойства
    public User Owner { get; private set; }
    public ICollection<DocumentCollaborator> Collaborators { get; private set; } = new List<DocumentCollaborator>();

    // Конструктор
    public Document(string title, Guid ownerId, string s3Path, bool isPrivate)
    {
        Id = Guid.NewGuid();
        Title = title;
        OwnerId = ownerId;
        S3Path = s3Path;
        IsPrivate = isPrivate;
        LastEdited = DateTime.UtcNow;
    }

    private Document() { } // Для EF Core

    public void UpdateS3Path(string newS3Path)
    {
        S3Path = newS3Path;
        LastEdited = DateTime.UtcNow;
    }
    
    public void UpdateTitle(string newTitle)
    {
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            Title = newTitle;
        }
    }
    
    public void UpdateLastEdited()
    {
        LastEdited = DateTime.UtcNow;
    }
    
    public void SetPrivacy(bool isPrivate)
    {
        IsPrivate = isPrivate;
    }
}