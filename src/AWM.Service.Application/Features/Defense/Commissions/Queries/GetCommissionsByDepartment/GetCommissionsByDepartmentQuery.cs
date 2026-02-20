namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionsByDepartment;

using AWM.Service.Application.Features.Defense.Commissions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all commissions for a department in a given academic year.
/// </summary>
public sealed record GetCommissionsByDepartmentQuery : IRequest<Result<IReadOnlyList<CommissionDto>>>
{
    /// <summary>
    /// Department ID to filter by.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID to filter by.
    /// </summary>
    public int AcademicYearId { get; init; }
}
