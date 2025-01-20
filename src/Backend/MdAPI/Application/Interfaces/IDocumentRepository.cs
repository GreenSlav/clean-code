using Domain.Entities;

namespace Application.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<string?> GetUserRoleAsync(Guid documentId, Guid userId);
    Task<List<DocumentWithRole>> GetCollaboratorDocumentsAsync(Guid userId);

    Task AddCollaboratorAsync(Guid documentId, Guid userId, string role);
    
    Task<bool> IsCollaboratorAsync(Guid documentId, Guid userId);
    Task<DocumentCollaborator?> GetCollaboratorAsync(Guid documentId, Guid userId);
    Task<IEnumerable<Document>> GetByOwnerIdAsync(Guid ownerId);
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task RemoveCollaboratorsAsync(Guid documentId);
    Task DeleteAsync(Guid documentId);
}