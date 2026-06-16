using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Queries.GetEmployeesStatus;

public sealed record GetEmployeesStatusQuery(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result<EmployeesStatusDto>>;
