using System;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Attachments;

public record AttachmentResponse(
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
    int UploadedBy,
    string? DownloadUrl = null);
