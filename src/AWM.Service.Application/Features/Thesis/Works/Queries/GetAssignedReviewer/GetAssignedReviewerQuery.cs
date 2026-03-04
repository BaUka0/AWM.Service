namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetAssignedReviewer;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetAssignedReviewerQuery : IRequest<Result<AssignedReviewerDto?>>
{
    public long WorkId { get; init; }
}

public sealed record AssignedReviewerDto
{
    public long ReviewId { get; init; }
    public int ReviewerId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Position { get; init; }
    public string? Organization { get; init; }
    public string? Email { get; init; }
    public bool IsUploaded { get; init; }
}
