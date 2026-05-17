namespace AWM.Service.Application.Features.Org.Departments.Queries.GetAllDepartments;

using AWM.Service.Application.Features.Org.Departments.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all departments belonging to a specific university.
/// </summary>
public sealed record GetAllDepartmentsQuery : IRequest<Result<IReadOnlyList<DepartmentDto>>>;
