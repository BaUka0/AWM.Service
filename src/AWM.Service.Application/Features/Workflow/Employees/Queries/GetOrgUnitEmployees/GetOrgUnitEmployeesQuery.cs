using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Queries.GetOrgUnitEmployees;

public sealed record GetOrgUnitEmployeesQuery(int OrgUnitId) : IRequest<Result<IReadOnlyList<TeacherDto>>>;
