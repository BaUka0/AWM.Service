namespace AWM.Service.Infrastructure.FileStorage;

using System.Security.Cryptography;
using AWM.Service.Domain.Thesis.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// File storage implementation that saves files to the local file system.
/// Suitable for development and single-server deployments.
/// Configured via "FileStorage:LocalBasePath" in appsettings.json.
/// </summary>
/// <remarks>
/// <b>TEMPORARY IMPLEMENTATION.</b> Will be replaced by a MinIO-based storage service
/// once the MinIO MVP is ready. Do not use in production deployments.
/// See <see cref="S3FileStorageService"/> for the planned production implementation.
/// </remarks>
[Obsolete("Temporary local storage implementation. Will be replaced by MinIO-based storage.")]
public sealed class LocalFileStorageService : IAttachmentService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _basePath = configuration["FileStorage:LocalBasePath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        Directory.CreateDirectory(_basePath);
    }

    /// <inheritdoc />
    public async Task<string> SaveAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(fileStream);

        // Build a unique path: uploads/{year}/{month}/{guid}{ext}
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var relativePath = Path.Combine(
            DateTime.UtcNow.Year.ToString(),
            DateTime.UtcNow.Month.ToString("D2"),
            $"{Guid.NewGuid()}{extension}");

        var fullPath = Path.Combine(_basePath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var outputStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(outputStream, cancellationToken);

        _logger.LogInformation("Saved attachment to local path: {RelativePath}", relativePath);

        // Return the relative path as the storage key
        return relativePath.Replace(Path.DirectorySeparatorChar, '/');
    }

    /// <inheritdoc />
    public Task DeleteAsync(string fileStoragePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileStoragePath);

        var fullPath = Path.Combine(_basePath, fileStoragePath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted attachment at local path: {StoragePath}", fileStoragePath);
        }
        else
        {
            _logger.LogWarning("Attempted to delete non-existent file: {StoragePath}", fileStoragePath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Stream> GetAsync(string fileStoragePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileStoragePath);

        var fullPath = Path.Combine(_basePath, fileStoragePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Attachment not found at path: {fileStoragePath}", fullPath);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    /// <inheritdoc />
    public async Task<string> ComputeHashAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        fileStream.Position = 0;
        var hashBytes = await SHA256.HashDataAsync(fileStream, cancellationToken);
        return Convert.ToHexString(hashBytes);
    }
}
