using Domain.Entities;

namespace Application.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<IEnumerable<Document>> GetByOwnerIdAsync(Guid ownerId);
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
}