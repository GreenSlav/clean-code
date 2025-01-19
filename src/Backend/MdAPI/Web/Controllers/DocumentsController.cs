using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.BodyModels;
using Web.Extensions;

namespace Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly DocumentService _documentService;

    public DocumentsController(DocumentService documentService)
    {
        _documentService = documentService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDocument(Guid id)
    {
        var userId = this.GetUserId(); // ✅ Получаем ID пользователя из JWT

        var (success, message, content, role) = await _documentService.GetDocumentAsync(id, userId);

        if (!success)
        {
            return Forbid(); // ❌ Запрещаем доступ
        }

        return Ok(new GetDocumentResponse { Content = content, Role = role });
    }

    // Создание документа
    // ✅ Создание нового документа
    [HttpPost("create")]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        var userId = this.GetUserId(); // Получаем ID пользователя из JWT

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title cannot be empty." });
        }

        var (success, message, documentId) = await _documentService.CreateDocumentAsync(
            userId, request.Title, request.Content, request.IsPrivate);

        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message = "Document created successfully.", documentId });
    }

    // Обновление документа
    [HttpPut]
    public async Task<IActionResult> UpdateDocument([FromBody] UpdateDocumentRequest request)
    {
        var userId = this.GetUserId();
        var (success, message) = await _documentService.UpdateDocumentAsync(request.DocumentId, userId, request.Content);

        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message = "Document updated successfully." });
    }
}