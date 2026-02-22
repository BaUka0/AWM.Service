namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksBySupervisor;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all student works supervised by a specific staff member.
/// </summary>
public sealed record GetStudentWorksBySupervisorQuery : IRequest<Result<IReadOnlyList<StudentWorkDto>>>
{
    /// <summary>
    /// Supervisor (Staff) ID.
    /// </summary>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }
}
