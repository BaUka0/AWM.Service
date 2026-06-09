namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;

/// <summary>
/// Attachment entity - file metadata for work attachments.
/// Actual files are stored externally (S3/FileServer).
/// </summary>
public class Attachment : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int? StateId { get; private set; }
    public int AttachmentTypeId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string FileStoragePath { get; private set; } = null!;
    public string FileHash { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public string ContentType { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public AttachmentType? AttachmentType { get; private set; }

    public int UploadedBy => CreatedBy;
    public DateTime UploadedAt => CreatedAt;

    private Attachment() { }

    internal Attachment(
        long workId,
        int? stateId,
        int attachmentTypeId,
        string fileName,
        string fileStoragePath,
        string fileHash,
        int uploadedBy,
        long fileSizeBytes,
        string contentType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("Attachment.FileNameRequired", "File name is required.");
        if (string.IsNullOrWhiteSpace(fileStoragePath))
            throw new DomainException("Attachment.FileStoragePathRequired", "File storage path is required.");
        if (string.IsNullOrWhiteSpace(fileHash))
            throw new DomainException("Attachment.FileHashRequired", "File hash is required.");
        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException("Attachment.ContentTypeRequired", "Content type is required.");

        WorkId = workId;
        StateId = stateId;
        AttachmentTypeId = attachmentTypeId;
        FileName = fileName;
        FileStoragePath = fileStoragePath;
        FileHash = fileHash.ToUpperInvariant();
        FileSizeBytes = fileSizeBytes;
        ContentType = contentType;

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
