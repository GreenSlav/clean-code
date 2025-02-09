namespace Web.BodyModels;

public class AddCollaboratorRequest
{
    public string Username { get; set; }
    public string Role { get; set; } // "viewer" / "editor"
}