namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Request contract for uploading an external review.
/// </summary>
public sealed record UploadReviewRequest
{
    /// <summary>
    /// Content of the external review (optional if file is provided).
    /// </summary>
    /// <example>Работа заслуживает высокой оценки...</example>
    public string? ReviewText { get; init; }

    /// <summary>
    /// Attached file (e.g., scanned signed copy). Required if text is null.
    /// </summary>
    public IFormFile? File { get; init; }
}
