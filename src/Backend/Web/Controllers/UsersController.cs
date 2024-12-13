using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        var (success, message) = await _userService.RegisterAsync(username, email, password);
        if (!success) return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var (success, message) = await _userService.LoginAsync(email, password);
        if (!success) return Unauthorized(new { message });

        return Ok(new { message });
    }
}