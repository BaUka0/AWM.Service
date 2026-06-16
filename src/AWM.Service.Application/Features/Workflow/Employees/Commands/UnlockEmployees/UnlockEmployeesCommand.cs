using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.UnlockEmployees;

public sealed record UnlockEmployeesCommand(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result>;
