namespace AWM.Service.Application.Features.Edu.Employees.Queries.GetSupervisors;

using AWM.Service.Application.Features.Edu.Employees.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetSupervisorsQuery : IRequest<Result<IReadOnlyList<EmployeeDto>>>
{
    public int DepartmentId { get; init; }
}
