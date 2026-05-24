using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.UpdateSupervisorWorkload;

public sealed record UpdateSupervisorWorkloadCommand(
    int DepartmentId,
    int UserId,
    int SemesterId,
    int? SpecialityId,
    int MaxWorkload
) : IRequest<Result<Unit>>;
