namespace AWM.Service.Application.Features.Edu.Employees.Queries.GetEmployeesByDepartment;

using AWM.Service.Application.Features.Edu.Employees.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetEmployeesByDepartmentQuery : IRequest<Result<IReadOnlyList<EmployeeDto>>>
{
    public int DepartmentId { get; init; }
}
