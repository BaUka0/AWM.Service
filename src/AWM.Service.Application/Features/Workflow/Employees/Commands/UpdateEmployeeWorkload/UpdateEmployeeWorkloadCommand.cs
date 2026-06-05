using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.UpdateEmployeeWorkload;

public sealed record UpdateEmployeeWorkloadCommand(
    int OrgUnitId,
    int UserId,
    int SemesterId,
    int? SpecialityId,
    int MaxWorkload
) : IRequest<Result<Unit>>;
