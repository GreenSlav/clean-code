using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.BodyModels;

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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "All fields are required." });
        }

        var (success, message) = await _userService.RegisterAsync(request.Username, request.Email, request.Password);
        if (!success)
        {
            return BadRequest(new { message });
        }

        // Логика для автоматического логина
        var (loginSuccess, loginMessage, user) = await _userService.LoginAsync(request.Email, request.Password);
        if (!loginSuccess || user == null)
        {
            return BadRequest(new { message = "Registration succeeded, but automatic login failed." });
        }

        // Генерация токена
        var token = _authService.GenerateToken(user.Id, user.Username, user.Role);

        // Добавление токена в куки
        Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true, // Защищает от доступа через JavaScript
            Secure = true, // Требует HTTPS
            SameSite = SameSiteMode.Strict, // Только для одного сайта
            Expires = DateTime.UtcNow.AddHours(1) // Срок действия токена
        });

        return Ok(new { message = "User registered and logged in successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Both identifier and password are required." });
        }

        var (success, message, user) = await _userService.LoginAsync(request.Identifier, request.Password);
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
            Secure = true, // Требует HTTPS
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