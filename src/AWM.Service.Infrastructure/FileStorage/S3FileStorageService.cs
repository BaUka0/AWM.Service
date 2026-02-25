namespace AWM.Service.Infrastructure.FileStorage;

using System.Security.Cryptography;
using AWM.Service.Application.Features.Thesis.Attachments.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// AWS S3 implementation of IAttachmentService.
/// Configured via the "FileStorage:S3" section in appsettings.json:
/// <code>
/// "FileStorage": {
///   "S3": {
///     "BucketName": "awm-attachments",
///     "Region":     "us-east-1",
///     "KeyPrefix":  "attachments/"
///   }
/// }
/// </code>
///
/// NuGet dependency required (not yet added to project):
///   AWSSDK.S3 (Amazon.S3)
///
/// To activate, replace the LocalFileStorageService registration in
/// DependencyInjection.cs with:
///   services.AddScoped&lt;IAttachmentService, S3FileStorageService&gt;();
/// </summary>
public sealed class S3FileStorageService : IAttachmentService
{
    // ──────────────────────────────────────────────────────────────────────────
    // Configuration keys
    // ──────────────────────────────────────────────────────────────────────────
    private readonly string _bucketName;
    private readonly string _keyPrefix;

    // Kept as object to avoid a hard compile-time dependency on AWSSDK.S3.
    // Replace with IAmazonS3 once the NuGet package is installed.
    private readonly object _s3Client;

    private readonly ILogger<S3FileStorageService> _logger;

    public S3FileStorageService(IConfiguration configuration, ILogger<S3FileStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _bucketName = configuration["FileStorage:S3:BucketName"]
            ?? throw new InvalidOperationException("FileStorage:S3:BucketName is not configured.");

        _keyPrefix = configuration["FileStorage:S3:KeyPrefix"] ?? "attachments/";

        // ── Uncomment once AWSSDK.S3 is installed ────────────────────────────
        // var region = RegionEndpoint.GetBySystemName(
        //     configuration["FileStorage:S3:Region"] ?? "us-east-1");
        // _s3Client = new AmazonS3Client(region);
        // ─────────────────────────────────────────────────────────────────────

        _s3Client = new object(); // placeholder — remove when SDK is added
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

        // ── Uncomment once AWSSDK.S3 is installed ────────────────────────────
        // var s3 = (IAmazonS3)_s3Client;
        // var request = new PutObjectRequest
        // {
        //     BucketName  = _bucketName,
        //     Key         = key,
        //     InputStream = fileStream,
        //     ContentType = contentType,
        //     AutoCloseStream = false
        // };
        // await s3.PutObjectAsync(request, cancellationToken);
        // ─────────────────────────────────────────────────────────────────────

        await Task.CompletedTask; // remove once SDK call above is enabled
        return key;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string fileStoragePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileStoragePath);

        _logger.LogInformation("Deleting S3 object '{Key}' from bucket '{Bucket}'", fileStoragePath, _bucketName);

        // ── Uncomment once AWSSDK.S3 is installed ────────────────────────────
        // var s3 = (IAmazonS3)_s3Client;
        // await s3.DeleteObjectAsync(_bucketName, fileStoragePath, cancellationToken);
        // ─────────────────────────────────────────────────────────────────────

        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<Stream> GetAsync(string fileStoragePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileStoragePath);

        _logger.LogInformation("Downloading S3 object '{Key}' from bucket '{Bucket}'", fileStoragePath, _bucketName);

        // ── Uncomment once AWSSDK.S3 is installed ────────────────────────────
        // var s3 = (IAmazonS3)_s3Client;
        // var response = await s3.GetObjectAsync(_bucketName, fileStoragePath, cancellationToken);
        // return response.ResponseStream;
        // ─────────────────────────────────────────────────────────────────────

        await Task.CompletedTask;
        throw new NotImplementedException("S3FileStorageService.GetAsync requires AWSSDK.S3. " +
            "Install the 'AWSSDK.S3' NuGet package and uncomment the implementation above.");
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
