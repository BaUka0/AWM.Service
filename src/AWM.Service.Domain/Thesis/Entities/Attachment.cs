namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Attachment entity - file metadata for work attachments.
/// Actual files are stored externally (S3/FileServer).
/// </summary>
public class Attachment : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int? StateId { get; private set; }
    public AttachmentType AttachmentType { get; private set; }
    public string FileName { get; private set; } = null!;
    public string FileStoragePath { get; private set; } = null!;
    public string FileHash { get; private set; } = null!;
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Legacy fields for backward compatibility/DB mapping
    public int UploadedBy => CreatedBy;
    public DateTime UploadedAt => CreatedAt;

    private Attachment() { }

    internal Attachment(
        long workId,
        int? stateId,
        AttachmentType attachmentType,
        string fileName,
        string fileStoragePath,
        string fileHash,
        int uploadedBy)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(fileStoragePath))
            throw new ArgumentException("File storage path is required.", nameof(fileStoragePath));
        if (string.IsNullOrWhiteSpace(fileHash))
            throw new ArgumentException("File hash is required.", nameof(fileHash));

        WorkId = workId;
        StateId = stateId;
        AttachmentType = attachmentType;
        FileName = fileName;
        FileStoragePath = fileStoragePath;
        FileHash = fileHash.ToUpperInvariant();
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = uploadedBy;
    }

    /// <summary>
    /// Gets the file hash as a value object.
    /// </summary>
    public FileHash GetFileHashValue()
    {
        return Primitives.FileHash.Create(FileHash);
    }

    /// <summary>
    /// Verifies if a given hash matches this attachment's hash.
    /// </summary>
    public bool VerifyHash(string hash)
    {
        return string.Equals(FileHash, hash, StringComparison.OrdinalIgnoreCase);
    }
}
