namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetDefenseReadiness;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetDefenseReadinessQuery : IRequest<Result<DefenseReadinessDto>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

public sealed record DefenseReadinessDto
{
    public int TotalWorks { get; init; }
    public int FullyReady { get; init; }
    public int NotReady { get; init; }
    public IReadOnlyList<StudentReadinessItem> Items { get; init; } = [];
}

public sealed record StudentReadinessItem
{
    public long WorkId { get; init; }
    public bool PreDefensePassed { get; init; }
    public bool NormControlPassed { get; init; }
    public bool SoftwareCheckPassed { get; init; }
    public bool AntiPlagiarismPassed { get; init; }
    public bool HasReview { get; init; }
    public bool IsFullyReady { get; init; }
}
