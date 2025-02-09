namespace Web.BodyModels;

public class UpdateDocumentRequest
{
    public Guid DocumentId { get; set; }
    public string Content { get; set; } = string.Empty;
}