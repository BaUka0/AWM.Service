namespace AWM.Service.Application.Features.Defense.PreDefense.Queries.GetFailedPreDefenseStudents;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetFailedPreDefenseStudentsQuery : IRequest<Result<IReadOnlyList<FailedPreDefenseStudentDto>>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public int? PreDefenseNumber { get; init; }
}

public sealed record FailedPreDefenseStudentDto
{
    public long WorkId { get; init; }
    public int LastAttemptNumber { get; init; }
    public decimal? LastScore { get; init; }
    public bool CanRetake { get; init; }
    public DateTime LastAttemptDate { get; init; }
}
