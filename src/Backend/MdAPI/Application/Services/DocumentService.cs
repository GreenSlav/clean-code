using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class DocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageRepository _fileStorageRepository;
    private readonly IUserRepository _userRepository;

    public DocumentService(IDocumentRepository documentRepository, IFileStorageRepository fileStorageRepository, IUserRepository userRepository)
    {
        _documentRepository = documentRepository;
        _fileStorageRepository = fileStorageRepository;
        _userRepository = userRepository;
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
    
    public async Task<(bool Success, object? Settings, string Message)> GetDocumentSettingsAsync(Guid documentId, Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, null, "Document not found.");
        }

        // ✅ Проверяем, является ли пользователь владельцем или коллаборатором
        var collaborator = await _documentRepository.GetCollaboratorAsync(documentId, userId);
        if (document.OwnerId != userId && collaborator == null)
        {
            return (false, null, "Access denied.");
        }

        var collaborators = await _documentRepository.GetCollaboratorsAsync(documentId);

        var settings = new
        {
            Title = document.Title,
            IsPrivate = document.IsPrivate,
            Collaborators = collaborators.Select(c => new
            {
                Id = c.UserId,
                Username = c.User.Username,
                Role = c.Role
            }).ToList()
        };

        return (true, settings, "Document settings retrieved successfully.");
    }
    
    public async Task<(bool Success, object? NewCollaborator, string Message)> AddCollaboratorAsync(Guid documentId, Guid ownerId, string username, string role)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, null, "Document not found.");
        }

        // ✅ Только владелец документа может добавлять коллабораторов
        if (document.OwnerId != ownerId)
        {
            return (false, null, "Access denied.");
        }

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return (false, null, "User not found.");
        }

        // ✅ Проверяем, не является ли пользователь уже коллаборатором
        var existingCollaborator = await _documentRepository.GetCollaboratorAsync(documentId, user.Id);
        if (existingCollaborator != null)
        {
            return (false, null, "User is already a collaborator.");
        }

        // ✅ Добавляем нового коллаборатора
        await _documentRepository.AddCollaboratorAsync(documentId, user.Id, role);

        var newCollaborator = new
        {
            Id = user.Id,
            Username = user.Username,
            Role = role
        };

        return (true, newCollaborator, "Collaborator added successfully.");
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
    
    public async Task<(bool Success, string Message)> UpdateDocumentSettingsAsync(Guid documentId, Guid ownerId, UpdateDocumentSettingsRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found.");
        }

        // ✅ Только владелец документа может обновлять настройки
        if (document.OwnerId != ownerId)
        {
            return (false, "Access denied.");
        }

        // ✅ Обновляем название и приватность
        document.UpdateTitle(request.Title);
        document.SetPrivacy(request.IsPrivate);
        await _documentRepository.UpdateAsync(document);

        // ✅ Обновляем роли коллабораторов
        foreach (var collab in request.Collaborators)
        {
            var existingCollab = await _documentRepository.GetCollaboratorAsync(documentId, collab.Id);
            if (existingCollab != null)
            {
                existingCollab.UpdateRole(collab.Role);
                await _documentRepository.UpdateCollaboratorAsync(existingCollab);
            }
        }

        return (true, "Document settings updated successfully.");
    }

// ✅ Обновление документа
    // public async Task<(bool Success, string Message)> UpdateDocumentAsync(Guid documentId, Guid userId, string content)
    // {
    //     var document = await _documentRepository.GetByIdAsync(documentId);
    //     if (document == null || document.OwnerId != userId)
    //     {
    //         return (false, "Access denied.");
    //     }
    //
    //     string newS3Path = await _fileStorageRepository.UploadFileAsync(userId, content);
    //     document.UpdateS3Path(newS3Path);
    //     await _documentRepository.UpdateAsync(document);
    //
    //     return (true, "Document updated successfully.");
    // }
    
    public async Task<(bool Success, string Message)> UpdateDocumentAsync(Guid documentId, Guid userId, string content)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found.");
        }

        if (document.OwnerId != userId && !await _documentRepository.IsCollaboratorAsync(documentId, userId))
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
    
    public async Task<(bool Success, string Message)> RemoveCollaboratorAsync(Guid documentId, Guid ownerId, Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            return (false, "Document not found.");
        }

        // ✅ Только владелец может удалять коллабораторов
        if (document.OwnerId != ownerId)
        {
            return (false, "Access denied.");
        }

        var collaborator = await _documentRepository.GetCollaboratorAsync(documentId, userId);
        if (collaborator == null)
        {
            return (false, "Collaborator not found.");
        }

        // ✅ Удаляем коллаборатора
        await _documentRepository.RemoveCollaboratorAsync(collaborator);

        return (true, "Collaborator removed successfully.");
    }
}