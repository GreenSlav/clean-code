namespace Application.Interfaces;

public interface IFileStorageRepository
{
    Task<string> UploadFileAsync(Guid userId, string content);
    Task<string?> GetFileAsync(Guid userId, string fileName);
    Task<string> DownloadFileAsync(string s3Path);
    Task DeleteFileAsync(Guid userId, string fileName);
    Task DeleteFileAsync(string s3Path);
}