namespace Web.BodyModels;

public class LoginRequest
{
    public string Identifier { get; set; } // Может быть Email или Username
    public string Password { get; set; }
}