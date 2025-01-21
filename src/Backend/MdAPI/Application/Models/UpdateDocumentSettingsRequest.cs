namespace Domain.Entities;

public class UpdateDocumentSettingsRequest
{
    public string Title { get; set; }
    public bool IsPrivate { get; set; }
    public List<CollaboratorUpdate> Collaborators { get; set; }
}