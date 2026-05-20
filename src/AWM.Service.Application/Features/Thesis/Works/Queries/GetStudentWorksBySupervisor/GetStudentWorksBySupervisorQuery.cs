namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksBySupervisor;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all student works supervised by a specific staff member.
/// </summary>
public sealed record GetStudentWorksBySupervisorQuery : IRequest<Result<(IReadOnlyList<StudentWorkDto> Items, int TotalCount)>>
{
    /// <summary>
    /// Supervisor (Staff) ID.
    /// </summary>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Items per page.
    /// </summary>
    public int PageSize { get; init; } = 10;
}
