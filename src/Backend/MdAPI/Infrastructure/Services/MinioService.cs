using Application.Interfaces;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System.Text;
using Minio.DataModel.Args;

namespace Infrastructure.Persistence.Repositories;

public class MinioFileStorageRepository : IFileStorageRepository
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName = "documents"; // Бакет для хранения MD-файлов

    public MinioFileStorageRepository(string endpoint, string accessKey, string secretKey)
    {
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    // Проверяем, есть ли бакет
    private async Task EnsureBucketExistsAsync()
    {
        var found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }
    }

    // Загрузка файла в MinIO
    public async Task<string> UploadFileAsync(Guid userId, string content)
    {
        await EnsureBucketExistsAsync();

        string fileName = $"{Guid.NewGuid()}.md"; 
        string objectName = $"{userId}/{fileName}"; 

        byte[] data = Encoding.UTF8.GetBytes(content);
        using var stream = new MemoryStream(data);

        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(data.Length)
            .WithContentType("text/markdown");

        await _minioClient.PutObjectAsync(args);
        return objectName;
    }

    // Получение файла
    public async Task<string?> GetFileAsync(Guid userId, string fileName)
    {
        string objectName = $"{userId}/{fileName}";

        using var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(async stream =>
            {
                await stream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
            });

        await _minioClient.GetObjectAsync(args);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
    
    public async Task<string> DownloadFileAsync(string s3Path)
    {
        using var memoryStream = new MemoryStream();
    
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket("documents")
            .WithObject(s3Path)
            .WithCallbackStream(async stream =>
            {
                await stream.CopyToAsync(memoryStream);
            }));

        memoryStream.Seek(0, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    // Удаление файла
    public async Task DeleteFileAsync(Guid userId, string fileName)
    {
        string objectName = $"{userId}/{fileName}";
        var args = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(args);
    }
}