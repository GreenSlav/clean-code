using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public AuthController(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        var (success, message) = await _userService.RegisterAsync(username, email, password);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string identifier, string password)
    {
        var (success, message, user) = await _userService.LoginAsync(identifier, password);
        if (!success || user == null)
        {
            return Unauthorized(new { message });
        }

        // Генерируем JWT токен
        var token = _authService.GenerateToken(user.Id, user.Username, user.Role);

        // Добавляем токен в куки
        Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true, // Защищает от доступа через JavaScript
            Secure = true,   // Требует HTTPS
            SameSite = SameSiteMode.Strict, // Только для одного сайта
            Expires = DateTime.UtcNow.AddHours(1) // Срок действия токена
        });

        return Ok(new { message = "Login successful." });
    }
    
    [Authorize] // Требует JWT токен
    [HttpGet("verify")]
    public IActionResult Verify()
    {
        // Если пользователь авторизован, просто возвращаем 200 OK
        return Ok(new { message = "User is authenticated." });
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return Ok(new { message = "Logout successful." });
    }
}