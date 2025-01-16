using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlazorController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;

    public BlazorController(IWebHostEnvironment env)
    {
        _env = env;
        _contentTypeProvider = new FileExtensionContentTypeProvider();
    }

    [HttpGet("resource/{*filename}")] // Поддержка вложенных путей
    public IActionResult GetBlazorResource(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return BadRequest("Filename cannot be empty.");
        }

        // Определяем путь к файлу
        var filePath = Path.Combine(_env.WebRootPath, "wwwroot/_framework", filename);

        // Проверяем существование файла
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { message = "Resource not found." });
        }

        // Определяем MIME-тип файла
        if (!_contentTypeProvider.TryGetContentType(filename, out var contentType))
        {
            contentType = "application/octet-stream"; // Если не определен MIME-тип, используем по умолчанию
        }

        // Включаем кэширование для статических файлов
        Response.Headers["Cache-Control"] = "public, max-age=31536000";

        // Возвращаем файл с MIME-типом
        return PhysicalFile(filePath, contentType);
    }
}