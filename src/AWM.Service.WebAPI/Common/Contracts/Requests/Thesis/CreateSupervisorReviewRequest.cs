namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Request contract for creating or updating a supervisor review.
/// </summary>
public sealed record CreateSupervisorReviewRequest
{
    /// <summary>
    /// Content of the review.
    /// </summary>
    /// <example>Студент показал отличные результаты...</example>
    public string ReviewText { get; init; } = null!;

    /// <summary>
    /// Optional attached file (e.g., scanned signed copy).
    /// </summary>
    public IFormFile? File { get; init; }
}
