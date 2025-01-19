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

    // Создание документа
    [HttpPost]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        var userId = this.GetUserId();
        var (success, message, documentId) = await _documentService.CreateDocumentAsync(userId, request.Title, request.Content);

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