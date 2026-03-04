namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetReviewStatusByDepartment;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetReviewStatusByDepartmentQuery : IRequest<Result<ReviewStatusByDepartmentDto>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

public sealed record ReviewStatusByDepartmentDto
{
    public int TotalWorks { get; init; }
    public int WorksWithReviewer { get; init; }
    public int WorksWithoutReviewer { get; init; }
    public int ReviewsUploaded { get; init; }
    public int ReviewsPending { get; init; }
    public IReadOnlyList<WorkReviewStatusItem> Items { get; init; } = [];
}

public sealed record WorkReviewStatusItem
{
    public long WorkId { get; init; }
    public int? ReviewerId { get; init; }
    public string? ReviewerName { get; init; }
    public bool HasReviewer { get; init; }
    public bool IsReviewUploaded { get; init; }
}
