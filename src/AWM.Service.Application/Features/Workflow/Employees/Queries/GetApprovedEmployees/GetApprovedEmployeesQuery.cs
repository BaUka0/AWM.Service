using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Queries.GetApprovedEmployees;

public sealed record GetApprovedEmployeesQuery(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result<IReadOnlyList<TeacherDto>>>;
