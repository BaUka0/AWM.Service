namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.UploadReview;

using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.AspNetCore.Http;

public sealed record UploadReviewCommand : IRequest<Result>
{
    public long ReviewId { get; init; }

    /// <summary>
    /// Student work ID to verify review ownership.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// Content of the external review (optional if file is provided).
    /// </summary>
    public string? ReviewText { get; init; }

    /// <summary>
    /// Attached file (e.g., scanned signed copy). Required if text is null.
    /// </summary>
    public IFormFile? File { get; init; }
}
