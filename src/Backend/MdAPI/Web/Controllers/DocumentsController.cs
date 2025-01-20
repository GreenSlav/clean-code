using System.Text;
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

    [HttpGet]
    public async Task<IActionResult> GetUserDocuments()
    {
        var userId = this.GetUserId(); // ✅ Получаем ID пользователя из JWT

        var documents = await _documentService.GetUserDocumentsAsync(userId);
    
        return Ok(documents.Select(d => new
        {
            id = d.Document.Id,  // ✅ Теперь правильно
            title = d.Document.Title,
            lastEdited = d.Document.LastEdited.ToString("dd.MM.yyyy HH:mm:ss"), 
            role = d.Document.OwnerId == userId ? "owner" : d.Role
        }));
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

    [Authorize]
    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> DownloadDocument(Guid documentId)
    {
        var userId = this.GetUserId();

        var (success, message, content) = await _documentService.DownloadDocumentAsync(documentId, userId);

        if (!success)
        {
            return Forbid();
        }

        var fileBytes = Encoding.UTF8.GetBytes(content);
        return File(fileBytes, "text/markdown", "document.md");
    }

    // Обновление документа
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentRequest request)
    {
        var userId = this.GetUserId(); // ✅ Получаем ID пользователя из JWT

        if (id != request.DocumentId) // ✅ Проверяем, совпадает ли ID документа
        {
            return BadRequest(new { message = "Document ID mismatch." });
        }

        var (success, message) = await _documentService.UpdateDocumentAsync(id, userId, request.Content);

        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message = "Document updated successfully." });
    }

    [Authorize]
    [HttpDelete("{documentId}")]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var userId = this.GetUserId();

        var (success, message) = await _documentService.DeleteDocumentAsync(documentId, userId);

        if (!success)
        {
            return Forbid();
        }

        return Ok(new { message = "Document deleted successfully." });
    }
}