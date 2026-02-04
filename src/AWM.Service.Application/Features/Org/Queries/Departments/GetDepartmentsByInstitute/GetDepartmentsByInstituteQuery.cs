namespace AWM.Service.Application.Features.Org.Queries.Departments.GetDepartmentsByInstitute;

using AWM.Service.Application.Features.Org.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all departments belonging to a specific institute.
/// </summary>
public sealed record GetDepartmentsByInstituteQuery : IRequest<Result<IReadOnlyList<DepartmentDto>>>
{
    public int InstituteId { get; init; }
}