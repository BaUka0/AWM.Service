using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.RemoveSupervisor;

public sealed record RemoveSupervisorCommand(
    int DepartmentId,
    int UserId,
    int SemesterId,
    int? SpecialityId
) : IRequest<Result<Unit>>;
