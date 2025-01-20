using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public DocumentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<string?> GetUserRoleAsync(Guid documentId, Guid userId)
    {
        var collaborator = await _dbContext.DocumentCollaborators
            .FirstOrDefaultAsync(dc => dc.DocumentId == documentId && dc.UserId == userId);

        return collaborator?.Role; // `owner`, `editor`, `viewer` или `null`, если доступа нет
    }
    
    public async Task<List<DocumentWithRole>> GetCollaboratorDocumentsAsync(Guid userId)
    {
        return await _dbContext.DocumentCollaborators
            .Where(dc => dc.UserId == userId)
            .Join(_dbContext.Documents,
                dc => dc.DocumentId,
                doc => doc.Id,
                (dc, doc) => new DocumentWithRole(doc, dc.Role)) // ✅ Создаём объект
            .ToListAsync();
    }

    public async Task AddCollaboratorAsync(Guid documentId, Guid userId, string role)
    {
        var collaborator = new DocumentCollaborator(documentId, userId, role);

        await _dbContext.DocumentCollaborators.AddAsync(collaborator);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<bool> IsCollaboratorAsync(Guid documentId, Guid userId)
    {
        return await _dbContext.DocumentCollaborators
            .AnyAsync(dc => dc.DocumentId == documentId && dc.UserId == userId);
    }
    
    public async Task<DocumentCollaborator?> GetCollaboratorAsync(Guid documentId, Guid userId)
    {
        return await _dbContext.DocumentCollaborators
            .FirstOrDefaultAsync(dc => dc.DocumentId == documentId && dc.UserId == userId);
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Documents
            .Include(d => d.Collaborators) // Подгружаем коллабораторов (если нужно)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _dbContext.Documents
            .Where(d => d.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task AddAsync(Document document)
    {
        await _dbContext.Documents.AddAsync(document);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Document document)
    {
        _dbContext.Documents.Update(document);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task RemoveCollaboratorsAsync(Guid documentId)
    {
        var collaborators = _dbContext.DocumentCollaborators.Where(dc => dc.DocumentId == documentId);
        _dbContext.DocumentCollaborators.RemoveRange(collaborators);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid documentId)
    {
        var document = await _dbContext.Documents.FindAsync(documentId);
        if (document != null)
        {
            _dbContext.Documents.Remove(document);
            await _dbContext.SaveChangesAsync();
        }
    }
}