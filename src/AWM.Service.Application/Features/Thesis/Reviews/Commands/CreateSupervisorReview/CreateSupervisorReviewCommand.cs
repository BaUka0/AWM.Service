namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.CreateSupervisorReview;

using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.AspNetCore.Http;

public sealed record CreateSupervisorReviewCommand : IRequest<Result<long>>
{
    public long WorkId { get; init; }

    /// <summary>
    /// Content of the review.
    /// </summary>
    public string ReviewText { get; init; } = null!;

    /// <summary>
    /// Optional attached file (e.g., scanned signed copy).
    /// </summary>
    public IFormFile? File { get; init; }
}
