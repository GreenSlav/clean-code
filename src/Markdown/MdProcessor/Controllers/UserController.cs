using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using MdProcessor.DbContexts;


namespace MdProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public UsersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Получить пользователя по индексу
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        
        if (user == null)
            return NotFound();  // Вернем 404, если пользователь не найден
        
        return Ok(user);  // Вернем пользователя в формате JSON
    }
}