namespace MdProcessorWebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.StaticFiles;

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

        [HttpGet("resource/{filename}")]
        public IActionResult GetBlazorResource(string filename)
        {
            // Определяем путь к файлу в wwwroot/_framework или другой папке
            var filePath = Path.Combine(_env.WebRootPath, "wwwroot/_framework", filename);

            // Проверяем существование файла
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Определяем MIME-тип файла
            if (!_contentTypeProvider.TryGetContentType(filename, out var contentType))
            {
                contentType = "application/octet-stream"; // Если не определен MIME-тип, используем по умолчанию
            }

            // Возвращаем файл с определенным MIME-типом
            return PhysicalFile(filePath, contentType);
        }
    }
}