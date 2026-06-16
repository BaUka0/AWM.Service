namespace AWM.Service.Infrastructure.FileStorage;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// AWS S3 / MinIO implementation of IAttachmentService.
/// Configured via the "FileStorage:S3" section in appsettings.json:
/// </summary>
public sealed class S3FileStorageService : IAttachmentService
{
    private readonly string _bucketName;
    private readonly string _keyPrefix;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3FileStorageService> _logger;

    public S3FileStorageService(IOptions<StorageSettings> storageOptions, ILogger<S3FileStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var settings = storageOptions?.Value?.S3
            ?? throw new InvalidOperationException("FileStorage:S3 settings are not configured.");

        _bucketName = settings.BucketName;
        if (string.IsNullOrWhiteSpace(_bucketName))
        {
            throw new InvalidOperationException("FileStorage:S3:BucketName is not configured.");
        }

        _keyPrefix = settings.KeyPrefix ?? "attachments/";

        var config = new AmazonS3Config();

        if (!string.IsNullOrWhiteSpace(settings.ServiceUrl))
        {
            config.ServiceURL = settings.ServiceUrl;
            config.ForcePathStyle = settings.ForcePathStyle;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(
                string.IsNullOrWhiteSpace(settings.Region) ? "us-east-1" : settings.Region);
        }

        if (!string.IsNullOrWhiteSpace(settings.AccessKey) && !string.IsNullOrWhiteSpace(settings.SecretKey))
        {
            _s3Client = new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
        }
        else
        {
            _s3Client = new AmazonS3Client(config);
        }
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

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var key = $"{_keyPrefix}{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}{extension}";

        _logger.LogInformation("Uploading attachment to S3 bucket '{Bucket}' with key '{Key}'", _bucketName, key);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
        return key;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string fileStoragePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileStoragePath);

        _logger.LogInformation("Deleting S3 object '{Key}' from bucket '{Bucket}'", fileStoragePath, _bucketName);

        await _s3Client.DeleteObjectAsync(_bucketName, fileStoragePath, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stream> GetAsync(string fileStoragePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileStoragePath);

        _logger.LogInformation("Downloading S3 object '{Key}' from bucket '{Bucket}'", fileStoragePath, _bucketName);

        var response = await _s3Client.GetObjectAsync(_bucketName, fileStoragePath, cancellationToken);
        return response.ResponseStream;
    }

    /// <inheritdoc />
    public async Task<string> ComputeHashAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }
        var hashBytes = await SHA256.HashDataAsync(fileStream, cancellationToken);
        return Convert.ToHexString(hashBytes);
    }
}
