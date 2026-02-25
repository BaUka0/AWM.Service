namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using AWM.Service.Domain.Thesis.Enums;
using Microsoft.AspNetCore.Http;

/// <summary>
/// Request contract for uploading a file attachment (multipart/form-data).
/// </summary>
public sealed record UploadAttachmentRequest
{
    /// <summary>
    /// Type of attachment being uploaded (0 = Draft, 1 = Final, 2 = Presentation, 3 = Software).
    /// </summary>
    /// <example>1</example>
    public AttachmentType AttachmentType { get; init; }

    /// <summary>
    /// The file to upload.
    /// </summary>
    public IFormFile File { get; init; } = null!;
}
