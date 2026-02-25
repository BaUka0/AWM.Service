namespace AWM.Service.Application.Features.Thesis.Reviews.DTOs;

public sealed record WorkReviewsDto
{
    public SupervisorReviewDto? SupervisorReview { get; init; }
    public IReadOnlyList<ReviewDto> Reviews { get; init; } = Array.Empty<ReviewDto>();
}
