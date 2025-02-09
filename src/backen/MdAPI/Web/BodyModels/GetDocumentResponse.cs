namespace Web.BodyModels;

public class GetDocumentResponse
{
    public string Content { get; set; } = string.Empty;
    public string Role { get; set; } = "none"; // "owner", "editor", "viewer", "none"
}