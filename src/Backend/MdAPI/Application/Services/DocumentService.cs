using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class DocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageRepository _fileStorageRepository;

    public DocumentService(IDocumentRepository documentRepository, IFileStorageRepository fileStorageRepository)
    {
        _documentRepository = documentRepository;
        _fileStorageRepository = fileStorageRepository;
    }

    // Создание документа
    public async Task<(bool Success, string Message, Guid? DocumentId)> CreateDocumentAsync(
        Guid userId, string title, string content, bool isPrivate)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return (false, "Title cannot be empty.", null);
        }

        string s3Path = await _fileStorageRepository.UploadFileAsync(userId, content);

        // 📝 Создаём объект документа
        var document = new Document(title, content, userId, s3Path, isPrivate);
        await _documentRepository.AddAsync(document);

        return (true, "Document created successfully.", document.Id);
    }

    // Обновление документа
    public async Task<(bool Success, string Message)> UpdateDocumentAsync(Guid documentId, Guid userId, string content)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found.");
        }

        if (document.OwnerId != userId)
        {
            return (false, "Access denied.");
        }

        // Обновляем содержимое в MinIO
        string newS3Path = await _fileStorageRepository.UploadFileAsync(userId, content);
        document.UpdateContent(content, newS3Path);
        await _documentRepository.UpdateAsync(document);
        
        return (true, "Document updated successfully.");
    }
}