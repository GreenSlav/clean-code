namespace Application.Models;

public class DocumentSettingsResponse
{
    public string Title { get; set; }
    public bool IsPrivate { get; set; }
    public List<CollaboratorDto> Collaborators { get; set; } = new();
    public string RequesterRole { get; set; } // 🔥 Добавили роль запрашивающего
}