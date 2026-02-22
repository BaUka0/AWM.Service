namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksByDepartment;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all student works in a department for an academic year.
/// Used by department administrators and secretaries.
/// </summary>
public sealed record GetStudentWorksByDepartmentQuery : IRequest<Result<IReadOnlyList<StudentWorkDto>>>
{
    /// <summary>
    /// Department ID.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }
}
