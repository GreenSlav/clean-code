namespace Domain.Entities;

public class DocumentWithRole
{
    public Document Document { get; set; }
    public string Role { get; set; }

    public DocumentWithRole(Document document, string role)
    {
        Document = document;
        Role = role;
    }
}