namespace AWM.Service.Domain.Common;

/// <summary>
/// Settings for file storage configuration.
/// </summary>
public sealed class StorageSettings
{
    public const string SectionName = "FileStorage";

    public string Provider { get; set; } = "Local";
    public string LocalBasePath { get; set; } = "uploads";
    public int MaxAttachmentSizeMb { get; set; } = 50;
    public int MaxReviewSizeMb { get; set; } = 10;
    public S3Settings S3 { get; set; } = new();
}

/// <summary>
/// Settings specifically for AWS S3 / MinIO storage.
/// </summary>
public sealed class S3Settings
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string KeyPrefix { get; set; } = "attachments/";
    public string ServiceUrl { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool ForcePathStyle { get; set; } = false;
}
