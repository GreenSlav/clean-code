namespace Application.Interfaces;

public interface IFileStorageRepository
{
    Task<string> UploadFileAsync(Guid userId, string content);
    Task<string?> GetFileAsync(Guid userId, string fileName);
    Task DeleteFileAsync(Guid userId, string fileName);
}