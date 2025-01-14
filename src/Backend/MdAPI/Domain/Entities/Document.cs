namespace Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public Guid OwnerId { get; private set; }

    public Document(string title, string content, Guid ownerId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Content = content;
        OwnerId = ownerId;
    }
}