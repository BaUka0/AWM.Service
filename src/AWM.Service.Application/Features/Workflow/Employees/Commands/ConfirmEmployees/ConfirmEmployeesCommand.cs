using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.ConfirmEmployees;

public sealed record ConfirmEmployeesCommand(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result>;
