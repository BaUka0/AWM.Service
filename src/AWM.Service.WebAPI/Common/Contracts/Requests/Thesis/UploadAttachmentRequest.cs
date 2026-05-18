namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Request contract for uploading a file attachment (multipart/form-data).
/// </summary>
public sealed record UploadAttachmentRequest
{
    /// <summary>
    /// Type of attachment being uploaded (1 = Draft, 2 = Final, 3 = Presentation, 4 = Software, 5 = Demo, 6 = Handout).
    /// </summary>
    /// <example>1</example>
    public int AttachmentType { get; init; }

    /// <summary>
    /// The file to upload.
    /// </summary>
    public IFormFile File { get; init; } = null!;
}
