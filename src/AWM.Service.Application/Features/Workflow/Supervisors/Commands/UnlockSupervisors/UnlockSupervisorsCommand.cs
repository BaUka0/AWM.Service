using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.UnlockSupervisors;

public sealed record UnlockSupervisorsCommand(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result>;
