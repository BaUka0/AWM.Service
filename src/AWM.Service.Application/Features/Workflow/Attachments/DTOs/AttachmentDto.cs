namespace AWM.Service.Application.Features.Workflow.Attachments.DTOs;

public record AttachmentDto(
    long Id,
    long WorkId,
    int? StateId,
    int AttachmentTypeId,
    string AttachmentTypeName,
    string FileName,
    long FileSizeBytes,
    string ContentType,
    string FileHash,
    DateTime UploadedAt,
    int UploadedBy);
