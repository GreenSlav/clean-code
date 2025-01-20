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

    public async Task<(bool Success, string Message, string Content, string Role)> GetDocumentAsync(Guid documentId,
        Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found.", "", "none");
        }

        // ✅ Если пользователь = владелец, он "owner"
        if (document.OwnerId == userId)
        {
            var content = await _fileStorageRepository.DownloadFileAsync(document.S3Path);
            return (true, "Owner access granted.", content, "owner");
        }

        // ✅ Получаем роль пользователя из `DocumentCollaborators`
        var collaboratorRole = await _documentRepository.GetUserRoleAsync(documentId, userId);
        if (collaboratorRole != null)
        {
            var content = await _fileStorageRepository.DownloadFileAsync(document.S3Path);
            return (true, "Collaborator access granted.", content, collaboratorRole);
        }
        
        // ✅ Если документ публичный, разрешаем просмотр (роль `viewer`)
        if (!document.IsPrivate)
        {
            var content = await _fileStorageRepository.DownloadFileAsync(document.S3Path);
            return (true, "Public document.", content, "viewer");
        }

        return (false, "Access denied.", "", "none"); // ❌ Доступ запрещён
    }
    
    public async Task<List<DocumentWithRole>> GetUserDocumentsAsync(Guid userId)
    {
        // ✅ Получаем все документы, где пользователь = владелец
        var ownedDocs = await _documentRepository.GetByOwnerIdAsync(userId);

        // ✅ Получаем все документы, где пользователь = `editor` или `viewer`
        var collaboratedDocs = await _documentRepository.GetCollaboratorDocumentsAsync(userId);

        return ownedDocs.Select(doc => new DocumentWithRole(doc, "owner"))
            .Concat(collaboratedDocs)
            .ToList();
    }

// ✅ Создание документа
    public async Task<(bool Success, string Message, Guid? DocumentId)> CreateDocumentAsync(Guid userId, string title,
        string content, bool isPrivate)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return (false, "Title cannot be empty.", null);
        }

        string s3Path = await _fileStorageRepository.UploadFileAsync(userId, content);

        var document = new Document(title, userId, s3Path, isPrivate);
        await _documentRepository.AddAsync(document);

        // ✅ Добавляем владельца в `DocumentCollaborators` с ролью `owner`
        await _documentRepository.AddCollaboratorAsync(document.Id, userId, "owner");

        return (true, "Document created successfully.", document.Id);
    }
    
    public async Task<(bool Success, string Message, string Content)> DownloadDocumentAsync(Guid documentId, Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found.", "");
        }

        // ✅ Проверяем доступ
        var hasAccess = document.OwnerId == userId || await _documentRepository.IsCollaboratorAsync(documentId, userId);
        if (!hasAccess)
        {
            return (false, "Access denied.", "");
        }

        // ✅ Загружаем содержимое из MinIO
        var content = await _fileStorageRepository.DownloadFileAsync(document.S3Path);
        return (true, "Success", content);
    }

// ✅ Обновление документа
    public async Task<(bool Success, string Message)> UpdateDocumentAsync(Guid documentId, Guid userId, string content)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null || document.OwnerId != userId)
        {
            return (false, "Access denied.");
        }

        string newS3Path = await _fileStorageRepository.UploadFileAsync(userId, content);
        document.UpdateS3Path(newS3Path);
        await _documentRepository.UpdateAsync(document);

        return (true, "Document updated successfully.");
    }
    
    public async Task<(bool Success, string Message)> DeleteDocumentAsync(Guid documentId, Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found.");
        }

        // ✅ Только владелец (`owner`) может удалить документ
        if (document.OwnerId != userId)
        {
            return (false, "Access denied.");
        }

        // ✅ Удаляем файл из MinIO
        await _fileStorageRepository.DeleteFileAsync(document.S3Path);

        // ✅ Удаляем всех коллабораторов
        await _documentRepository.RemoveCollaboratorsAsync(documentId);

        // ✅ Удаляем сам документ
        await _documentRepository.DeleteAsync(documentId);

        return (true, "Document deleted successfully.");
    }
}