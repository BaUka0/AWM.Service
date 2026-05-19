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
    /// Org unit ID to filter by.
    /// </summary>
    public int OrgUnitId { get; init; }

    /// <summary>
    /// Semester ID to filter by.
    /// </summary>
    public int SemesterId { get; init; }
}
